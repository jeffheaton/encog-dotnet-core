using System;
using System.IO;
using Encog.App.Analyst.CSV;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Util;
using Encog.ML;
using Encog.Persist;
using Encog.Util.Logging;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    /// This class is used to evaluate a machine learning method. Evaluation data is
    /// provided and the ideal and actual responses from the machine learning method
    /// are written to a file.
    /// </summary>
    ///
    public class CmdEvaluateRaw : Cmd
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        ///
        public const String CommandName = "EVALUATE-RAW";

        /// <summary>
        /// Construct an evaluate raw command.
        /// </summary>
        ///
        /// <param name="analyst">The analyst object to use.</param>
        public CmdEvaluateRaw(EncogAnalyst analyst) : base(analyst)
        {
        }

        /// <inheritdoc/>
        public override String Name
        {
            get { return CommandName; }
        }

        /// <inheritdoc/>
        public override sealed bool ExecuteCommand(String args)
        {
            // get filenames
            String evalID = Prop.GetPropertyString(
                ScriptProperties.MlConfigEvalFile);
            String resourceID = Prop.GetPropertyString(
                ScriptProperties.MlConfigMachineLearningFile);

            String outputID = Prop.GetPropertyString(
                ScriptProperties.MlConfigOutputFile);

            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "Beginning evaluate raw");
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "evaluate file:" + evalID);
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "resource file:"
                                                       + resourceID);

            FileInfo evalFile = Script.ResolveFilename(evalID);
            FileInfo resourceFile = Script.ResolveFilename(resourceID);

            FileInfo outputFile = Analyst.Script.ResolveFilename(
                outputID);

            var method = (MLRegression) EncogDirectoryPersistence
                                            .LoadObject(resourceFile);

            bool headers = Script.ExpectInputHeaders(evalID);

            var eval = new AnalystEvaluateRawCSV {Script = Script};
            Analyst.CurrentQuantTask = eval;
            eval.Report = new AnalystReportBridge(Analyst);
            eval.Analyze(Analyst, evalFile, headers, Prop
                                                         .GetPropertyCSVFormat(ScriptProperties.SetupConfigCSVFormat));
            eval.Process(outputFile, method);
            Analyst.CurrentQuantTask = null;
            return eval.ShouldStop();
        }
    }
}