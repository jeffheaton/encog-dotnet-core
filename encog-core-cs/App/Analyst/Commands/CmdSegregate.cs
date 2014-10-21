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
    ///     This command is used to segregate one CSV file into several. This can be
    ///     useful for creating training and evaluation sets.
    /// </summary>
    public class CmdSegregate : Cmd
    {
        /// <summary>
        ///     The name of this command.
        /// </summary>
        public const String CommandName = "SEGREGATE";

        /// <summary>
        ///     Construct the segregate command.
        /// </summary>
        /// <param name="analyst">The analyst to use.</param>
        public CmdSegregate(EncogAnalyst analyst) : base(analyst)
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
                ScriptProperties.SegregateConfigSourceFile);

            FileInfo sourceFile = Script.ResolveFilename(sourceID);

            EncogLogging.Log(EncogLogging.LevelDebug, "Beginning segregate");
            EncogLogging.Log(EncogLogging.LevelDebug, "source file:" + sourceID);

            // get formats
            CSVFormat format = Script.DetermineFormat();

            // prepare to segregate
            bool headers = Script.ExpectInputHeaders(sourceID);
            var seg = new SegregateCSV {Script = Script};
            Analyst.CurrentQuantTask = seg;

            foreach (AnalystSegregateTarget target  in  Script.Segregate.SegregateTargets)
            {
                FileInfo filename = Script.ResolveFilename(target.File);
                seg.Targets.Add(new SegregateTargetPercent(filename, target.Percent));
                // mark generated
                Script.MarkGenerated(target.File);
                EncogLogging.Log(
                    EncogLogging.LevelDebug,
                    "target file:" + target.File + ", Percent: "
                    + Format.FormatPercent(target.Percent));
            }

            seg.Report = new AnalystReportBridge(Analyst);
            seg.Analyze(sourceFile, headers, format);

            seg.Process();
            Analyst.CurrentQuantTask = null;
            return seg.ShouldStop();
        }
    }
}
