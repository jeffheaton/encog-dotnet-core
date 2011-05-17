using System;
using System.IO;
using Encog.App.Analyst.CSV.Segregate;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Script.Segregate;
using Encog.App.Analyst.Util;
using Encog.Util;
using Encog.Util.CSV;
using Encog.Util.Logging;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    /// This command is used to segregate one CSV file into several. This can be
    /// useful for creating training and evaluation sets.
    /// </summary>
    ///
    public class CmdSegregate : Cmd
    {
        /// <summary>
        /// The name of this command.
        /// </summary>
        ///
        public const String COMMAND_NAME = "SEGREGATE";

        /// <summary>
        /// Construct the segregate command.
        /// </summary>
        ///
        /// <param name="analyst">The analyst to use.</param>
        public CmdSegregate(EncogAnalyst analyst) : base(analyst)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override String Name
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return COMMAND_NAME; }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed bool ExecuteCommand(String args)
        {
            // get filenames
            String sourceID = Prop.GetPropertyString(
                ScriptProperties.SEGREGATE_CONFIG_SOURCE_FILE);

            FileInfo sourceFile = Script.ResolveFilename(sourceID);

            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "Beginning segregate");
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "source file:" + sourceID);

            // get formats
            CSVFormat inputFormat = Script
                .DetermineInputFormat(sourceID);
            CSVFormat outputFormat = Script.DetermineOutputFormat();

            // prepare to segregate
            bool headers = Script.ExpectInputHeaders(sourceID);
            var seg = new SegregateCSV();
            seg.Script = Script;
            Analyst.CurrentQuantTask = seg;

            foreach (AnalystSegregateTarget target  in  Script.Segregate.SegregateTargets)
            {
                FileInfo filename = Script.ResolveFilename(target.File);
                seg.Targets.Add(new SegregateTargetPercent(filename, target.Percent));
                // mark generated
                Script.MarkGenerated(target.File);
                EncogLogging.Log(
                    EncogLogging.LEVEL_DEBUG,
                    "target file:" + target.File + ", Percent: "
                    + Format.FormatPercent(target.Percent));
            }

            seg.Report = new AnalystReportBridge(Analyst);
            seg.Analyze(sourceFile, headers, inputFormat);
            seg.OutputFormat = outputFormat;

            seg.Process();
            Analyst.CurrentQuantTask = null;
            return seg.ShouldStop();
        }
    }
}