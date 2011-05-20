using System;
using System.IO;
using Encog.App.Analyst.CSV.Shuffle;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Util;
using Encog.Util.CSV;
using Encog.Util.Logging;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    /// This command is used to randomize the lines in a CSV file.
    /// </summary>
    ///
    public class CmdRandomize : Cmd
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        ///
        public const String COMMAND_NAME = "RANDOMIZE";

        /// <summary>
        /// Construct the randomize command.
        /// </summary>
        ///
        /// <param name="analyst">The analyst to use.</param>
        public CmdRandomize(EncogAnalyst analyst) : base(analyst)
        {
        }

        /// <inheritdoc/>
        public override String Name
        {
            get { return COMMAND_NAME; }
        }

        /// <inheritdoc/>
        public override sealed bool ExecuteCommand(String args)
        {
            // get filenames
            String sourceID = Prop.GetPropertyString(
                ScriptProperties.RANDOMIZE_CONFIG_SOURCE_FILE);
            String targetID = Prop.GetPropertyString(
                ScriptProperties.RANDOMIZE_CONFIG_TARGET_FILE);

            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "Beginning randomize");
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "source file:" + sourceID);
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "target file:" + targetID);

            FileInfo sourceFile = Script.ResolveFilename(sourceID);
            FileInfo targetFile = Script.ResolveFilename(targetID);

            // get formats
            CSVFormat inputFormat = Script
                .DetermineInputFormat(sourceID);
            CSVFormat outputFormat = Script.DetermineOutputFormat();

            // mark generated
            Script.MarkGenerated(targetID);

            // prepare to normalize
            var norm = new ShuffleCSV();
            norm.Script = Script;
            Analyst.CurrentQuantTask = norm;
            norm.Report = new AnalystReportBridge(Analyst);
            bool headers = Script.ExpectInputHeaders(sourceID);
            norm.Analyze(sourceFile, headers, inputFormat);
            norm.OutputFormat = outputFormat;
            norm.Process(targetFile);
            Analyst.CurrentQuantTask = null;
            return norm.ShouldStop();
        }
    }
}