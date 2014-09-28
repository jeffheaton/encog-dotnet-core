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
    ///     Performs the balance command. This allows large classes to have members
    ///     discarded.
    /// </summary>
    public class CmdBalance : Cmd
    {
        /// <summary>
        ///     The name of this command.
        /// </summary>
        public const String CommandName = "BALANCE";

        /// <summary>
        ///     Construct the balance command.
        /// </summary>
        /// <param name="analyst">The analyst to use with this command.</param>
        public CmdBalance(EncogAnalyst analyst) : base(analyst)
        {
        }

        /// <inheritdoc />
        public override String Name
        {
            get { return CommandName; }
        }

        /// <inheritdoc />
        public override sealed bool ExecuteCommand(String args)
        {
            // get filenames
            String sourceID = Prop.GetPropertyString(
                ScriptProperties.BalanceConfigSourceFile);
            String targetID = Prop.GetPropertyString(
                ScriptProperties.BalanceConfigTargetFile);

            EncogLogging.Log(EncogLogging.LevelDebug, "Beginning balance");
            EncogLogging.Log(EncogLogging.LevelDebug, "source file:" + sourceID);
            EncogLogging.Log(EncogLogging.LevelDebug, "target file:" + targetID);

            FileInfo sourceFile = Script.ResolveFilename(sourceID);
            FileInfo targetFile = Script.ResolveFilename(targetID);

            // get other config data
            int countPer = Prop.GetPropertyInt(
                ScriptProperties.BalanceConfigCountPer);
            String targetFieldStr = Prop.GetPropertyString(
                ScriptProperties.BalanceConfigBalanceField);
            DataField targetFieldDf = Analyst.Script.FindDataField(
                targetFieldStr);
            if (targetFieldDf == null)
            {
                throw new AnalystError("Can't find balance target field: "
                                       + targetFieldStr);
            }
            if (!targetFieldDf.Class)
            {
                throw new AnalystError("Can't balance on non-class field: "
                                       + targetFieldStr);
            }

            int targetFieldIndex = Analyst.Script
                                          .FindDataFieldIndex(targetFieldDf);

            // mark generated
            Script.MarkGenerated(targetID);

            // get formats
            CSVFormat inputFormat = Script.DetermineFormat();
            CSVFormat outputFormat = Script.DetermineFormat();

            // prepare to normalize
            var balance = new BalanceCSV {Script = Script};
            Analyst.CurrentQuantTask = balance;
            balance.Report = new AnalystReportBridge(Analyst);

            bool headers = Script.ExpectInputHeaders(sourceID);
            balance.Analyze(sourceFile, headers, inputFormat);
            balance.ProduceOutputHeaders = true;
            balance.Process(targetFile, targetFieldIndex, countPer);
            Analyst.CurrentQuantTask = null;
            return balance.ShouldStop();
        }
    }
}
