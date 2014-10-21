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
using Encog.App.Analyst.CSV.Process;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Util;
using Encog.Util.CSV;
using Encog.Util.Logging;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    ///     This command is used to preprocess a CSV file.
    /// </summary>
    public class CmdProcess : Cmd
    {
        /// <summary>
        ///     The name of the command.
        /// </summary>
        public static String COMMAND_NAME = "PROCESS";

        /// <summary>
        ///     Construct the randomize command.
        /// </summary>
        /// <param name="analyst">The analyst to use.</param>
        public CmdProcess(EncogAnalyst analyst)
            : base(analyst)
        {
        }

        /// <inheritdoc />
        public override String Name
        {
            get { return COMMAND_NAME; }
        }

        /// <inheritdoc />
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
            var process = new AnalystProcess(Analyst, backwardSize, forwardSize);
            process.Script = Script;
            Analyst.CurrentQuantTask = process;
            process.Report = new AnalystReportBridge(Analyst);
            bool headers = Script.ExpectInputHeaders(sourceID);
            process.Analyze(sourceFile, headers, format);
            process.Process(targetFile);
            Analyst.CurrentQuantTask = null;
            return process.ShouldStop();
        }
    }
}
