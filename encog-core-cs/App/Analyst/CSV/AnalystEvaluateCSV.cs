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
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Util;
using Encog.App.Quant;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util.Arrayutil;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV
{
    /// <summary>
    ///     Used by the analyst to evaluate a CSV file.
    /// </summary>
    public class AnalystEvaluateCSV : BasicFile
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
        ///     The number of columns in the file.
        /// </summary>
        private int _fileColumns;

        /// <summary>
        ///     The number of output columns.
        /// </summary>
        private int _outputColumns;

        /// <summary>
        ///     Used to handle time series.
        /// </summary>
        private TimeSeriesUtil _series;

        /// <summary>
        ///     Analyze the data. This counts the records and prepares the data to be
        ///     processed.
        /// </summary>
        /// <param name="theAnalyst">The analyst to use.</param>
        /// <param name="inputFile">The input file.</param>
        /// <param name="headers">True if headers are present.</param>
        /// <param name="format">The format.</param>
        public void Analyze(EncogAnalyst theAnalyst,
                            FileInfo inputFile, bool headers, CSVFormat format)
        {
            InputFilename = inputFile;
            ExpectInputHeaders = headers;
            Format = format;

            Analyzed = true;
            _analyst = theAnalyst;

            PerformBasicCounts();
            _fileColumns = InputHeadings.Length;
            _outputColumns = _analyst.DetermineOutputFieldCount();

            _analystHeaders = new CSVHeaders(InputHeadings);
            _series = new TimeSeriesUtil(_analyst, false,
                                         _analystHeaders.Headers);
        }

        /// <summary>
        ///     Prepare the output file, write headers if needed.
        /// </summary>
        /// <param name="outputFile">The output file.</param>
        /// <returns>The file to write to.</returns>
        private new StreamWriter PrepareOutputFile(FileInfo outputFile)
        {
            try
            {
                outputFile.Delete();
                var tw = new StreamWriter(outputFile.OpenWrite());

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


                    // now add the output fields that will be generated
                    foreach (AnalystField field  in  _analyst.Script.Normalize.NormalizedFields)
                    {
                        if (field.Output && !field.Ignored)
                        {
                            AppendSeparator(line, Format);
                            line.Append("\"Output:");
                            line.Append(CSVHeaders.TagColumn(field.Name, 0,
                                                             field.TimeSlice, false));
                            line.Append("\"");
                        }
                    }

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
        ///     Process the file.
        /// </summary>
        /// <param name="outputFile">The output file.</param>
        /// <param name="method">THe method to use.</param>
        public void Process(FileInfo outputFile, IMLMethod method)
        {
            var csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, Format);

            IMLData output;

            foreach (AnalystField field in _analyst.Script.Normalize.NormalizedFields)
            {
                field.Init();
            }

            int outputLength = _analyst.DetermineTotalInputFieldCount();

            StreamWriter tw = PrepareOutputFile(outputFile);

            ResetStatus();
            while (csv.Next())
            {
                UpdateStatus(false);
                var row = new LoadedRow(csv, _outputColumns);

                double[] inputArray = AnalystNormalizeCSV.ExtractFields(_analyst,
                                                                        _analystHeaders, csv, outputLength, true);
                if (_series.TotalDepth > 1)
                {
                    inputArray = _series.Process(inputArray);
                }

                if (inputArray != null)
                {
                    IMLData input = new BasicMLData(inputArray);

                    // evaluation data
                    if ((method is IMLClassification)
                        && !(method is IMLRegression))
                    {
                        // classification only?
                        var tmp = new BasicMLData(1);
                        tmp[0] = ((IMLClassification) method).Classify(input);
                        output = tmp;
                    }
                    else
                    {
                        // regression
                        output = ((IMLRegression) method).Compute(input);
                    }

                    // skip file data
                    int index = _fileColumns;
                    int outputIndex = 0;


                    // display output
                    foreach (AnalystField field  in  _analyst.Script.Normalize.NormalizedFields)
                    {
                        if (_analystHeaders.Find(field.Name) != -1)
                        {
                            if (field.Output)
                            {
                                if (field.Classify)
                                {
                                    // classification
                                    ClassItem cls = field.DetermineClass(
                                        outputIndex, output);
                                    outputIndex += field.ColumnsNeeded;
                                    if (cls == null)
                                    {
                                        row.Data[index++] = "?Unknown?";
                                    }
                                    else
                                    {
                                        row.Data[index++] = cls.Name;
                                    }
                                }
                                else
                                {
                                    // regression
                                    double n = output[outputIndex++];
                                    n = field.DeNormalize(n);
                                    row.Data[index++] = Format
                                        .Format(n, Precision);
                                }
                            }
                        }
                    }
                }

                WriteRow(tw, row);
            }
            ReportDone(false);
            tw.Close();
            csv.Close();
        }
    }
}
