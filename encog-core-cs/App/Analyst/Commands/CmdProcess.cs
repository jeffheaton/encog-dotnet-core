using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Logging;
using Encog.App.Analyst.Script.Prop;
using System.IO;
using Encog.Util.CSV;
using Encog.App.Analyst.CSV.Process;
using Encog.App.Analyst.Util;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    /// This command is used to preprocess a CSV file.
    /// </summary>
    public class CmdProcess : Cmd
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        public static String COMMAND_NAME = "PROCESS";

        /// <summary>
        /// Construct the randomize command.
        /// </summary>
        /// <param name="analyst">The analyst to use.</param>
        public CmdProcess(EncogAnalyst analyst)
            : base(analyst)
        {
        }

        /// <inheritdoc/>
        public override bool ExecuteCommand(String args)
        {
            // get filenames
            String sourceID = Prop.GetPropertyString(
                    ScriptProperties.PROCESS_CONFIG_SOURCE_FILE);
            String targetID = Prop.GetPropertyString(
                    ScriptProperties.PROCESS_CONFIG_TARGET_FILE);

            int forwardSize = Prop.GetPropertyInt(
                    ScriptProperties.PROCESS_CONFIG_FORWARD_SIZE);
            int backwardSize = Prop.GetPropertyInt(
                    ScriptProperties.PROCESS_CONFIG_BACKWARD_SIZE);

            EncogLogging.Log(EncogLogging.LevelDebug, "Beginning randomize");
            EncogLogging.Log(EncogLogging.LevelDebug, "source file:" + sourceID);
            EncogLogging.Log(EncogLogging.LevelDebug, "target file:" + targetID);

            FileInfo sourceFile = Script.ResolveFilename(sourceID);
            FileInfo targetFile = Script.ResolveFilename(targetID);

            // get formats
            CSVFormat format = Script.DetermineFormat();

            // mark generated
            Script.MarkGenerated(targetID);

            // prepare to transform
            AnalystProcess process = new AnalystProcess(Analyst, backwardSize, forwardSize);
            process.Script = Script;
            Analyst.CurrentQuantTask = process;
            process.Report = new AnalystReportBridge(Analyst);
            bool headers = Script.ExpectInputHeaders(sourceID);
            process.Analyze(sourceFile, headers, format);
            process.Process(targetFile);
            Analyst.CurrentQuantTask = null;
            return process.ShouldStop();
        }

        /// <inheritdoc/>
        public override String Name
        {
            get
            {
                return CmdProcess.COMMAND_NAME;
            }
        }
    }
}
