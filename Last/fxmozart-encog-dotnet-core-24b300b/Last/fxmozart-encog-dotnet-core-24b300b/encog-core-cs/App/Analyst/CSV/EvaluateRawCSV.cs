using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using System.IO;
using Encog.Util.CSV;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.App.Quant;

namespace Encog.App.Analyst.CSV
{
    /// <summary>
    /// Used by non-analyst programs to evaluate a CSV file.
    /// </summary>
    ///
    public class EvaluateRawCSV : BasicFile
    {
        /// <summary>
        /// The ideal count.
        /// </summary>
        ///
        private int _idealCount;

        /// <summary>
        /// The input count.
        /// </summary>
        ///
        private int _inputCount;

        /// <summary>
        /// The output count.
        /// </summary>
        ///
        private int _outputCount;

        /// <summary>
        /// Analyze the data. This counts the records and prepares the data to be
        /// processed.
        /// </summary>
        ///
        /// <param name="inputFile">The input file.</param>
        /// <param name="headers">True if headers are present.</param>
        /// <param name="format">The format the file is in.</param>
        public void Analyze(IMLRegression method, FileInfo inputFile, bool headers, CSVFormat format)
        {
            InputFilename = inputFile;
            ExpectInputHeaders = headers;
            InputFormat = format;

            Analyzed = true;

            PerformBasicCounts();

            _inputCount = method.InputCount;
            _outputCount = method.OutputCount;
            _idealCount = Math.Max( InputHeadings.Length - _inputCount, 0);

            if ((InputHeadings.Length != _inputCount)
                && (InputHeadings.Length != (_inputCount + _outputCount)))
            {
                throw new AnalystError("Invalid number of columns("
                                       + InputHeadings.Length + "), must match input("
                                       + _inputCount + ") count or input+output("
                                       + (_inputCount + _outputCount) + ") count.");
            }
        }

        /// <summary>
        /// Prepare the output file, write headers if needed.
        /// </summary>
        ///
        /// <param name="outputFile">The name of the output file.</param>
        /// <returns>The output stream for the text file.</returns>
        private StreamWriter AnalystPrepareOutputFile(FileInfo outputFile)
        {
            try
            {
                var tw = new StreamWriter(outputFile.OpenWrite());
                // write headers, if needed
                if (ProduceOutputHeaders)
                {
                    var line = new StringBuilder();

                    // first, handle any input fields
                    if (_inputCount > 0)
                    {
                        for (int i = 0; i < _inputCount; i++)
                        {
                            BasicFile.AppendSeparator(line, InputFormat);
                            line.Append("\"");
                            line.Append("input:" + i);
                            line.Append("\"");
                        }
                    }
                   
                    // now, handle the ideal fields
                    if (_idealCount > 0)
                    {

                        for (int i = 0; i < _idealCount; i++)
                        {
                            BasicFile.AppendSeparator(line, InputFormat);
                            line.Append("\"");
                            line.Append("ideal:" + i);
                            line.Append("\"");
                        }
                    }

                    // now, handle the output fields
                    if (_outputCount > 0)
                    {

                        for (int i = 0; i < _outputCount; i++)
                        {
                            BasicFile.AppendSeparator(line, InputFormat);
                            line.Append("\"");
                            line.Append("output:" + i);
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
        /// Process the file.
        /// </summary>
        ///
        /// <param name="outputFile">The output file.</param>
        /// <param name="method">The method to use.</param>
        public void Process(FileInfo outputFile, IMLRegression method)
        {
            var csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, InputFormat);

            if (method.InputCount > _inputCount)
            {
                throw new AnalystError("This machine learning method has "
                                       + method.InputCount
                                       + " inputs, however, the data has " + _inputCount
                                       + " inputs.");
            }

            IMLData input = new BasicMLData(method.InputCount);

            StreamWriter tw = AnalystPrepareOutputFile(outputFile);

            ResetStatus();
            while (csv.Next())
            {
                UpdateStatus(false);
                var row = new LoadedRow(csv, _idealCount);

                int dataIndex = 0;
                // load the input data
                for (int i = 0; i < _inputCount; i++)
                {
                    String str = row.Data[i];
                    double d = InputFormat.Parse(str);
                    input[i] = d;
                    dataIndex++;
                }

                // do we need to skip the ideal values?
                dataIndex += _idealCount;

                // compute the result
                IMLData output = method.Compute(input);

                // display the computed result
                for (int i = 0; i < _outputCount; i++)
                {
                    double d = output[i];
                    row.Data[dataIndex++] = InputFormat.Format(d, Precision);
                }

                WriteRow(tw, row);
            }
            ReportDone(false);
            tw.Close();
            csv.Close();
        }
    }
}
