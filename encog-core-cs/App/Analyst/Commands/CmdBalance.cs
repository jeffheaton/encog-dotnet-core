using System;
using System.IO;
using Encog.App.Analyst.CSV.Balance;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Util;
using Encog.Util.CSV;
using Encog.Util.Logging;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    /// Performs the balance command. This allows large classes to have members
    /// discarded.
    /// </summary>
    ///
    public class CmdBalance : Cmd
    {
        /// <summary>
        /// The name of this command.
        /// </summary>
        ///
        public const String COMMAND_NAME = "BALANCE";

        /// <summary>
        /// Construct the balance command.
        /// </summary>
        ///
        /// <param name="analyst">The analyst to use with this command.</param>
        public CmdBalance(EncogAnalyst analyst) : base(analyst)
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
                ScriptProperties.BALANCE_CONFIG_SOURCE_FILE);
            String targetID = Prop.GetPropertyString(
                ScriptProperties.BALANCE_CONFIG_TARGET_FILE);

            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "Beginning balance");
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "source file:" + sourceID);
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "target file:" + targetID);

            FileInfo sourceFile = Script.ResolveFilename(sourceID);
            FileInfo targetFile = Script.ResolveFilename(targetID);

            // get other config data
            int countPer = Prop.GetPropertyInt(
                ScriptProperties.BALANCE_CONFIG_COUNT_PER);
            String targetFieldStr = Prop.GetPropertyString(
                ScriptProperties.BALANCE_CONFIG_BALANCE_FIELD);
            DataField targetFieldDF = Analyst.Script.FindDataField(
                targetFieldStr);
            if (targetFieldDF == null)
            {
                throw new AnalystError("Can't find balance target field: "
                                       + targetFieldStr);
            }
            if (!targetFieldDF.Class)
            {
                throw new AnalystError("Can't balance on non-class field: "
                                       + targetFieldStr);
            }

            int targetFieldIndex = Analyst.Script
                .FindDataFieldIndex(targetFieldDF);

            // mark generated
            Script.MarkGenerated(targetID);

            // get formats
            CSVFormat inputFormat = Script
                .DetermineInputFormat(sourceID);
            CSVFormat outputFormat = Script.DetermineOutputFormat();

            // prepare to normalize
            var balance = new BalanceCSV();
            balance.Script = Script;
            Analyst.CurrentQuantTask = balance;
            balance.Report = new AnalystReportBridge(Analyst);

            bool headers = Script.ExpectInputHeaders(sourceID);
            balance.Analyze(sourceFile, headers, inputFormat);
            balance.OutputFormat = outputFormat;
            balance.ProduceOutputHeaders = true;
            balance.Process(targetFile, targetFieldIndex, countPer);
            Analyst.CurrentQuantTask = null;
            return balance.ShouldStop();
        }
    }
}