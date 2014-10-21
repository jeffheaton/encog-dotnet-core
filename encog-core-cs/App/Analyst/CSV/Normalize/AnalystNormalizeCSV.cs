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
using Encog.App.Analyst.Missing;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Util;
using Encog.App.Quant;
using Encog.Util.Arrayutil;
using Encog.Util.CSV;
using Encog.Util.Logging;

namespace Encog.App.Analyst.CSV.Normalize
{
    /// <summary>
    ///     Normalize, or denormalize, a CSV file.
    /// </summary>
    public class AnalystNormalizeCSV : BasicFile
    {
        /// <summary>
        ///     The analyst to use.
        /// </summary>
        private EncogAnalyst _analyst;

        /// <summary>
        ///     THe headers.
        /// </summary>
        private CSVHeaders _analystHeaders;

        /// <summary>
        ///     Used to process time series.
        /// </summary>
        private TimeSeriesUtil _series;

        /// <summary>
        ///     Extract fields from a file into a numeric array for machine learning.
        /// </summary>
        /// <param name="analyst">The analyst to use.</param>
        /// <param name="headers">The headers for the input data.</param>
        /// <param name="csv">The CSV that holds the input data.</param>
        /// <param name="outputLength">The length of the returned array.</param>
        /// <param name="skipOutput">True if the output should be skipped.</param>
        /// <returns>The encoded data.</returns>
        public static double[] ExtractFields(EncogAnalyst analyst,
                                             CSVHeaders headers, ReadCSV csv,
                                             int outputLength, bool skipOutput)
        {
            var output = new double[outputLength];
            int outputIndex = 0;

            foreach (AnalystField stat in analyst.Script.Normalize.NormalizedFields)
            {
                stat.Init();
                if (stat.Action == NormalizationAction.Ignore)
                {
                    continue;
                }

                if (stat.Output && skipOutput)
                {
                    continue;
                }

                int index = headers.Find(stat.Name);
                String str = csv.Get(index);

                // is this an unknown value?
                if (str.Equals("?") || str.Length == 0)
                {
                    IHandleMissingValues handler = analyst.Script.Normalize.MissingValues;
                    double[] d = handler.HandleMissing(analyst, stat);

                    // should we skip the entire row
                    if (d == null)
                    {
                        return null;
                    }

                    // copy the returned values in place of the missing values
                    for (int i = 0; i < d.Length; i++)
                    {
                        output[outputIndex++] = d[i];
                    }
                }
                else
                {
                    // known value

                    if (stat.Action == NormalizationAction.Normalize)
                    {
                        double d = csv.Format.Parse(str.Trim());
                        d = stat.Normalize(d);
                        output[outputIndex++] = d;
                    }
                    else if (stat.Action == NormalizationAction.PassThrough)
                    {
                        double d = csv.Format.Parse(str);
                        output[outputIndex++] = d;
                    }
                    else
                    {
                        double[] d = stat.Encode(str.Trim());

                        foreach (double element in d)
                        {
                            output[outputIndex++] = element;
                        }
                    }
                }
            }

            return output;
        }

        /// <summary>
        ///     Analyze the file.
        /// </summary>
        /// <param name="inputFilename">The input file.</param>
        /// <param name="expectInputHeaders">True, if input headers are present.</param>
        /// <param name="inputFormat">The format.</param>
        /// <param name="theAnalyst">The analyst to use.</param>
        public void Analyze(FileInfo inputFilename,
                            bool expectInputHeaders, CSVFormat inputFormat,
                            EncogAnalyst theAnalyst)
        {
            InputFilename = inputFilename;
            Format = inputFormat;
            ExpectInputHeaders = expectInputHeaders;
            _analyst = theAnalyst;
            Analyzed = true;

            _analystHeaders = new CSVHeaders(inputFilename, expectInputHeaders,
                                             inputFormat);


            foreach (AnalystField field  in  _analyst.Script.Normalize.NormalizedFields)
            {
                field.Init();
            }

            _series = new TimeSeriesUtil(_analyst, true,
                                         _analystHeaders.Headers);
        }

        /// <summary>
        ///     Normalize the input file. Write to the specified file.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        public void Normalize(FileInfo file)
        {
            if (_analyst == null)
            {
                throw new EncogError(
                    "Can't normalize yet, file has not been analyzed.");
            }

            ReadCSV csv = null;
            StreamWriter tw = null;

            try
            {
                csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, Format);

                file.Delete();
                tw = new StreamWriter(file.OpenWrite());

                // write headers, if needed
                if (ProduceOutputHeaders)
                {
                    WriteHeaders(tw);
                }

                ResetStatus();
                int outputLength = _analyst.DetermineTotalColumns();

                // write file contents
                while (csv.Next() && !ShouldStop())
                {
                    UpdateStatus(false);

                    double[] output = ExtractFields(
                        _analyst, _analystHeaders, csv, outputLength,
                        false);

                    if (_series.TotalDepth > 1)
                    {
                        output = _series.Process(output);
                    }

                    if (output != null)
                    {
                        var line = new StringBuilder();
                        NumberList.ToList(Format, line, output);
                        tw.WriteLine(line);
                    }
                }
            }
            catch (IOException e)
            {
                throw new QuantError(e);
            }
            finally
            {
                ReportDone(false);
                if (csv != null)
                {
                    try
                    {
                        csv.Close();
                    }
                    catch (Exception ex)
                    {
                        EncogLogging.Log(ex);
                    }
                }

                if (tw != null)
                {
                    try
                    {
                        tw.Close();
                    }
                    catch (Exception ex)
                    {
                        EncogLogging.Log(ex);
                    }
                }
            }
        }

        /// <summary>
        ///     Set the source file. This is useful if you want to use pre-existing stats
        ///     to normalize something and skip the analyze step.
        /// </summary>
        /// <param name="file">The file to use.</param>
        /// <param name="headers">True, if headers are to be expected.</param>
        /// <param name="format">The format of the CSV file.</param>
        public void SetSourceFile(FileInfo file, bool headers,
                                  CSVFormat format)
        {
            InputFilename = file;
            ExpectInputHeaders = headers;
            Format = format;
        }

        /// <summary>
        ///     Write the headers.
        /// </summary>
        /// <param name="tw">The output stream.</param>
        private void WriteHeaders(StreamWriter tw)
        {
            var line = new StringBuilder();

            foreach (AnalystField stat  in  _analyst.Script.Normalize.NormalizedFields)
            {
                int needed = stat.ColumnsNeeded;

                for (int i = 0; i < needed; i++)
                {
                    AppendSeparator(line, Format);
                    line.Append('\"');
                    line.Append(CSVHeaders.TagColumn(stat.Name, i,
                                                     stat.TimeSlice, needed > 1));
                    line.Append('\"');
                }
            }
            tw.WriteLine(line.ToString());
        }
    }
}
