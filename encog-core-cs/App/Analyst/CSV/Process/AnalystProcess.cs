using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.ML.Prg;
using Encog.ML.Prg.Ext;
using System.IO;
using Encog.Util.CSV;
using Encog.App.Analyst.Script.Process;
using Encog.App.Quant;
using Encog.ML.Prg.ExpValue;

namespace Encog.App.Analyst.CSV.Process
{
    /// <summary>
    /// Perform many different types of transformations on a CSV.
    /// </summary>
    public class AnalystProcess : BasicFile
    {
        private EncogProgramContext programContext = new EncogProgramContext();
        private EncogProgramVariables programVariables = new EncogProgramVariables();
        private IList<EncogProgram> expressionFields = new List<EncogProgram>();
        private ProcessExtension extension;
        private EncogAnalyst analyst;
        private int forwardWindowSize;
        private int backwardWindowSize;


        /// <summary>
        /// Construct the object.
        /// </summary>
        /// <param name="theAnalyst">The analyst.</param>
        /// <param name="theBackwardWindowSize">The backward window size.</param>
        /// <param name="theForwardWindowSize">The forward window size.</param>
        public AnalystProcess(EncogAnalyst theAnalyst, int theBackwardWindowSize, int theForwardWindowSize)
        {
            this.analyst = theAnalyst;

            this.backwardWindowSize = theBackwardWindowSize;
            this.forwardWindowSize = theForwardWindowSize;
            StandardExtensions.CreateAll(this.programContext);
        }

        /// <summary>
        /// Analyze the neural network.
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

            this.expressionFields.Clear();
            this.extension = new ProcessExtension(this.Format);
            this.extension.register(this.programContext.Functions);

            foreach (ProcessField field in this.analyst.Script.Process.Fields)
            {
                EncogProgram prg = new EncogProgram(this.programContext, this.programVariables);
                prg.SetExtraData(ProcessExtension.EXTENSION_DATA_NAME, this.extension);
                prg.CompileExpression(field.Command);
                this.expressionFields.Add(prg);
            }
        }

        /// <summary>
        /// Get the next row from the underlying CSV file.
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
        /// Prepare the output file, write headers if needed.
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
                if (this.ProduceOutputHeaders)
                {
                    int index = 0;
                    StringBuilder line = new StringBuilder();

                    foreach (ProcessField field in this.analyst.Script.Process.Fields)
                    {
                        if (line.Length > 0)
                        {
                            line.Append(this.Format.Separator);
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
            StringBuilder line = new StringBuilder();

            foreach (EncogProgram prg in this.expressionFields)
            {
                ExpressionValue result = prg.Evaluate();

                BasicFile.AppendSeparator(line, this.Format);

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
        /// Process, and generate the output file.
        /// </summary>
        /// <param name="outputFile">The output file.</param>
        public void Process(FileInfo outputFile)
        {
            ValidateAnalyzed();

            ReadCSV csv = new ReadCSV(InputFilename.ToString(), ExpectInputHeaders, Format);
            LoadedRow row;

            StreamWriter tw = PrepareOutputFile(outputFile);
            this.extension.Init(csv,
                    this.forwardWindowSize,
                    this.backwardWindowSize);

            ResetStatus();
            while ((row = GetNextRow(csv)) != null)
            {
                this.extension.LoadRow(row);

                if (this.extension.IsDataReady())
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
