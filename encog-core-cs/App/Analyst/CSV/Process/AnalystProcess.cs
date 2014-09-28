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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.App.Analyst.Script.Process;
using Encog.App.Quant;
using Encog.ML.Prg;
using Encog.ML.Prg.ExpValue;
using Encog.ML.Prg.Ext;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV.Process
{
    /// <summary>
    ///     Perform many different types of transformations on a CSV.
    /// </summary>
    public class AnalystProcess : BasicFile
    {
        private readonly EncogAnalyst analyst;
        private readonly int backwardWindowSize;
        private readonly IList<EncogProgram> expressionFields = new List<EncogProgram>();
        private readonly int forwardWindowSize;
        private readonly EncogProgramContext programContext = new EncogProgramContext();
        private readonly EncogProgramVariables programVariables = new EncogProgramVariables();
        private ProcessExtension extension;


        /// <summary>
        ///     Construct the object.
        /// </summary>
        /// <param name="theAnalyst">The analyst.</param>
        /// <param name="theBackwardWindowSize">The backward window size.</param>
        /// <param name="theForwardWindowSize">The forward window size.</param>
        public AnalystProcess(EncogAnalyst theAnalyst, int theBackwardWindowSize, int theForwardWindowSize)
        {
            analyst = theAnalyst;

            backwardWindowSize = theBackwardWindowSize;
            forwardWindowSize = theForwardWindowSize;
            StandardExtensions.CreateAll(programContext);
        }

        /// <summary>
        ///     Analyze the neural network.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="headers">True, if there are headers.</param>
        /// <param name="format">The format of the CSV file.</param>
        public void Analyze(FileInfo inputFile, bool headers, CSVFormat format)
        {
            InputFilename = inputFile;
            ExpectInputHeaders = headers;
            Format = format;

            Analyzed = true;

            PerformBasicCounts();

            expressionFields.Clear();
            extension = new ProcessExtension(Format);
            extension.register(programContext.Functions);

            foreach (ProcessField field in analyst.Script.Process.Fields)
            {
                var prg = new EncogProgram(programContext, programVariables);
                prg.SetExtraData(ProcessExtension.EXTENSION_DATA_NAME, extension);
                prg.CompileExpression(field.Command);
                expressionFields.Add(prg);
            }
        }

        /// <summary>
        ///     Get the next row from the underlying CSV file.
        /// </summary>
        /// <param name="csv">The underlying CSV file.</param>
        /// <returns>The loaded row.</returns>
        private LoadedRow GetNextRow(ReadCSV csv)
        {
            if (csv.Next())
            {
                return new LoadedRow(csv);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Prepare the output file, write headers if needed.
        /// </summary>
        /// <param name="outputFile">The name of the output file.</param>
        /// <returns>The output stream for the text file.</returns>
        public StreamWriter PrepareOutputFile(FileInfo outputFile)
        {
            try
            {
                outputFile.Delete();
                var tw = new StreamWriter(outputFile.OpenWrite());

                // write headers, if needed
                if (ProduceOutputHeaders)
                {
                    int index = 0;
                    var line = new StringBuilder();

                    foreach (ProcessField field in analyst.Script.Process.Fields)
                    {
                        if (line.Length > 0)
                        {
                            line.Append(Format.Separator);
                        }
                        line.Append("\"");
                        line.Append(field.Name);
                        line.Append("\"");
                        index++;
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

        private void ProcessRow(StreamWriter tw)
        {
            var line = new StringBuilder();

            foreach (EncogProgram prg in expressionFields)
            {
                ExpressionValue result = prg.Evaluate();

                AppendSeparator(line, Format);

                if (result.IsString)
                {
                    line.Append("\"");
                }

                line.Append(result.ToStringValue());

                if (result.IsString)
                {
                    line.Append("\"");
                }
            }
            tw.WriteLine(line.ToString());
        }

        /// <summary>
        ///     Process, and generate the output file.
        /// </summary>
        /// <param name="outputFile">The output file.</param>
        public void Process(FileInfo outputFile)
        {
            ValidateAnalyzed();

            var csv = new ReadCSV(InputFilename.ToString(), ExpectInputHeaders, Format);
            LoadedRow row;

            StreamWriter tw = PrepareOutputFile(outputFile);
            extension.Init(csv,
                           forwardWindowSize,
                           backwardWindowSize);

            ResetStatus();
            while ((row = GetNextRow(csv)) != null)
            {
                extension.LoadRow(row);

                if (extension.IsDataReady())
                {
                    ProcessRow(tw);
                }
                UpdateStatus(false);
            }
            ReportDone(false);
            tw.Close();
        }
    }
}
