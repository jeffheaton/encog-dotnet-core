//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.IO;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.App.Analyst.CSV.Normalize;
using Encog.App.Analyst.Util;
using Encog.App.Quant;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Kmeans;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV
{
    /// <summary>
    ///     Used by the analyst to cluster a CSV file.
    /// </summary>
    public class AnalystClusterCSV : BasicFile
    {
        /// <summary>
        ///     The analyst to use.
        /// </summary>
        private EncogAnalyst _analyst;

        /// <summary>
        ///     The headers.
        /// </summary>
        private CSVHeaders _analystHeaders;

        /// <summary>
        ///     The training data used to send to KMeans.
        /// </summary>
        private BasicMLDataSet _data;

        /// <summary>
        ///     Analyze the data. This counts the records and prepares the data to be
        ///     processed.
        /// </summary>
        /// <param name="theAnalyst">The analyst to use.</param>
        /// <param name="inputFile">The input file to analyze.</param>
        /// <param name="headers">True, if the input file has headers.</param>
        /// <param name="format">The format of the input file.</param>
        public void Analyze(EncogAnalyst theAnalyst,
                            FileInfo inputFile, bool headers, CSVFormat format)
        {
            InputFilename = inputFile;
            ExpectInputHeaders = headers;
            Format = format;

            Analyzed = true;
            _analyst = theAnalyst;

            _data = new BasicMLDataSet();
            ResetStatus();
            int recordCount = 0;

            int outputLength = _analyst.DetermineTotalColumns();
            var csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, Format);
            ReadHeaders(csv);

            _analystHeaders = new CSVHeaders(InputHeadings);

            while (csv.Next() && !ShouldStop())
            {
                UpdateStatus(true);

                double[] inputArray = AnalystNormalizeCSV.ExtractFields(
                    _analyst, _analystHeaders, csv, outputLength, true);

                IMLData input = new BasicMLData(inputArray);
                _data.Add(new BasicMLDataPair(input));

                recordCount++;
            }
            RecordCount = recordCount;
            Count = csv.ColumnCount;

            ReadHeaders(csv);
            csv.Close();
            ReportDone(true);
        }

        /// <summary>
        ///     Prepare the output file, write headers if needed.
        /// </summary>
        /// <param name="outputFile">The output file.</param>
        /// <returns>The file to be written to.</returns>
        private new StreamWriter PrepareOutputFile(FileInfo outputFile)
        {
            try
            {
                var tw = new StreamWriter(outputFile.OpenRead());

                // write headers, if needed
                if (ProduceOutputHeaders)
                {
                    var line = new StringBuilder();


                    // handle provided fields, not all may be used, but all should
                    // be displayed
                    foreach (String heading  in  InputHeadings)
                    {
                        AppendSeparator(line, Format);
                        line.Append("\"");
                        line.Append(heading);
                        line.Append("\"");
                    }

                    // now the output fields that will be generated
                    line.Append("\"cluster\"");

                    tw.WriteLine(line.ToString());
                }

                return tw;
            }
            catch (IOException e)
            {
                throw new QuantError(e);
            }
        }

        /// <summary>
        ///     Process the file and cluster.
        /// </summary>
        /// <param name="outputFile">The output file.</param>
        /// <param name="clusters">The number of clusters.</param>
        /// <param name="theAnalyst">The analyst to use.</param>
        /// <param name="iterations">The number of iterations to use.</param>
        public void Process(FileInfo outputFile, int clusters,
                            EncogAnalyst theAnalyst, int iterations)
        {
            StreamWriter tw = PrepareOutputFile(outputFile);

            ResetStatus();

            var cluster = new KMeansClustering(clusters,
                                               _data);
            cluster.Iteration(iterations);

            int clusterNum = 0;

            foreach (IMLCluster cl  in  cluster.Clusters)
            {
                foreach (IMLData item  in  cl.Data)
                {
                    int clsIndex = item.Count;
                    var lr = new LoadedRow(Format, item, 1);
                    lr.Data[clsIndex] = "" + clusterNum;
                    WriteRow(tw, lr);
                }
                clusterNum++;
            }

            ReportDone(false);
            tw.Close();
        }
    }
}
