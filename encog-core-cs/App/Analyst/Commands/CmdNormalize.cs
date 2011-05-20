using System;
using System.IO;
using Encog.App.Analyst.CSV.Normalize;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Util;
using Encog.Util.CSV;
using Encog.Util.Logging;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    /// The normalize command is used to normalize data. Data normalization generally
    /// maps values from one number range to another, typically to -1 to 1.
    /// </summary>
    ///
    public class CmdNormalize : Cmd
    {
        /// <summary>
        /// The name of this command.
        /// </summary>
        ///
        public const String COMMAND_NAME = "NORMALIZE";

        /// <summary>
        /// Construct the normalize command.
        /// </summary>
        ///
        /// <param name="theAnalyst">The analyst to use.</param>
        public CmdNormalize(EncogAnalyst theAnalyst) : base(theAnalyst)
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
                ScriptProperties.NORMALIZE_CONFIG_SOURCE_FILE);
            String targetID = Prop.GetPropertyString(
                ScriptProperties.NORMALIZE_CONFIG_TARGET_FILE);

            FileInfo sourceFile = Script.ResolveFilename(sourceID);
            FileInfo targetFile = Script.ResolveFilename(targetID);

            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "Beginning normalize");
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "source file:" + sourceID);
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "target file:" + targetID);

            // mark generated
            Script.MarkGenerated(targetID);

            // get formats
            CSVFormat inputFormat = Script
                .DetermineInputFormat(sourceID);
            CSVFormat outputFormat = Script.DetermineOutputFormat();

            // prepare to normalize
            var norm = new AnalystNormalizeCSV();
            norm.Script = Script;
            Analyst.CurrentQuantTask = norm;
            norm.Report = new AnalystReportBridge(Analyst);

            bool headers = Script.ExpectInputHeaders(sourceID);
            norm.Analyze(sourceFile, headers, inputFormat, Analyst);
            norm.OutputFormat = outputFormat;
            norm.ProduceOutputHeaders = true;
            norm.Normalize(targetFile);
            Analyst.CurrentQuantTask = null;
            return norm.ShouldStop();
        }
    }
}