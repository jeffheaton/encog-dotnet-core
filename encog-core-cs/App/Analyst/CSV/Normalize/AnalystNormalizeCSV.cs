using System;
using System.IO;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Util;
using Encog.App.Quant;
using Encog.Util.Arrayutil;
using Encog.Util.CSV;
using Encog.Util.Logging;

namespace Encog.App.Analyst.CSV.Normalize
{
    /// <summary>
    /// Normalize, or denormalize, a CSV file.
    /// </summary>
    ///
    public class AnalystNormalizeCSV : BasicFile
    {
        /// <summary>
        /// The analyst to use.
        /// </summary>
        ///
        private EncogAnalyst analyst;

        /// <summary>
        /// THe headers.
        /// </summary>
        ///
        private CSVHeaders analystHeaders;

        /// <summary>
        /// Used to process time series.
        /// </summary>
        ///
        private TimeSeriesUtil series;

        /// <summary>
        /// Extract fields from a file into a numeric array for machine learning.
        /// </summary>
        ///
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

            foreach (AnalystField stat  in  analyst.Script.Normalize.NormalizedFields)
            {
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

                if (stat.Action == NormalizationAction.Normalize)
                {
                    double d = csv.Format.Parse(str);
                    d = stat.Normalize(d);
                    output[outputIndex++] = d;
                }
                else
                {
                    double[] d_0 = stat.Encode(str);

                    foreach (double element  in  d_0)
                    {
                        output[outputIndex++] = element;
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Analyze the file.
        /// </summary>
        ///
        /// <param name="inputFilename">The input file.</param>
        /// <param name="expectInputHeaders">True, if input headers are present.</param>
        /// <param name="inputFormat">The format.</param>
        /// <param name="theAnalyst">The analyst to use.</param>
        public void Analyze(FileInfo inputFilename,
                            bool expectInputHeaders, CSVFormat inputFormat,
                            EncogAnalyst theAnalyst)
        {
            InputFilename = inputFilename;
            InputFormat = inputFormat;
            ExpectInputHeaders = expectInputHeaders;
            analyst = theAnalyst;
            Analyzed = true;

            analystHeaders = new CSVHeaders(inputFilename, expectInputHeaders,
                                            inputFormat);


            foreach (AnalystField field  in  analyst.Script.Normalize.NormalizedFields)
            {
                field.Init();
            }

            series = new TimeSeriesUtil(analyst,
                                        analystHeaders.Headers);
        }

        /// <summary>
        /// Normalize the input file. Write to the specified file.
        /// </summary>
        ///
        /// <param name="file">The file to write to.</param>
        public void Normalize(FileInfo file)
        {
            if (analyst == null)
            {
                throw new EncogError(
                    "Can't normalize yet, file has not been analyzed.");
            }

            ReadCSV csv = null;
            StreamWriter tw = null;

            try
            {
                csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, InputFormat);

				file.Delete();
                tw = new StreamWriter(file.OpenWrite());

                // write headers, if needed
                if (ProduceOutputHeaders)
                {
                    WriteHeaders(tw);
                }

                ResetStatus();
                int outputLength = analyst.DetermineUniqueColumns();

                // write file contents
                while (csv.Next() && !ShouldStop())
                {
                    UpdateStatus(false);

                    double[] output = ExtractFields(
                        analyst, analystHeaders, csv, outputLength,
                        false);

                    if (series.TotalDepth > 1)
                    {
                        output = series.Process(output);
                    }

                    if (output != null)
                    {
                        var line = new StringBuilder();
                        NumberList.ToList(OutputFormat, line, output);
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
                    catch (Exception ex_0)
                    {
                        EncogLogging.Log(ex_0);
                    }
                }
            }
        }

        /// <summary>
        /// Set the source file. This is useful if you want to use pre-existing stats
        /// to normalize something and skip the analyze step.
        /// </summary>
        ///
        /// <param name="file">The file to use.</param>
        /// <param name="headers">True, if headers are to be expected.</param>
        /// <param name="format">The format of the CSV file.</param>
        public void SetSourceFile(FileInfo file, bool headers,
                                  CSVFormat format)
        {
            InputFilename = file;
            ExpectInputHeaders = headers;
            InputFormat = format;
        }

        /// <summary>
        /// Write the headers.
        /// </summary>
        ///
        /// <param name="tw">The output stream.</param>
        private void WriteHeaders(StreamWriter tw)
        {
            var line = new StringBuilder();

            foreach (AnalystField stat  in  analyst.Script.Normalize.NormalizedFields)
            {
                int needed = stat.ColumnsNeeded;

                for (int i = 0; i < needed; i++)
                {
                    AppendSeparator(line, InputFormat);
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