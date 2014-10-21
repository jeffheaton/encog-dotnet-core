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
using Encog.App.Analyst.CSV.Normalize;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Util;
using Encog.Util.CSV;
using Encog.Util.Logging;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    ///     The normalize command is used to normalize data. Data normalization generally
    ///     maps values from one number range to another, typically to -1 to 1.
    /// </summary>
    public class CmdNormalize : Cmd
    {
        /// <summary>
        ///     The name of this command.
        /// </summary>
        public const String CommandName = "NORMALIZE";

        /// <summary>
        ///     Construct the normalize command.
        /// </summary>
        /// <param name="theAnalyst">The analyst to use.</param>
        public CmdNormalize(EncogAnalyst theAnalyst) : base(theAnalyst)
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
                ScriptProperties.NormalizeConfigSourceFile);
            String targetID = Prop.GetPropertyString(
                ScriptProperties.NormalizeConfigTargetFile);

            FileInfo sourceFile = Script.ResolveFilename(sourceID);
            FileInfo targetFile = Script.ResolveFilename(targetID);

            EncogLogging.Log(EncogLogging.LevelDebug, "Beginning normalize");
            EncogLogging.Log(EncogLogging.LevelDebug, "source file:" + sourceID);
            EncogLogging.Log(EncogLogging.LevelDebug, "target file:" + targetID);

            // mark generated
            Script.MarkGenerated(targetID);

            // get formats
            CSVFormat format = Script.DetermineFormat();

            // prepare to normalize
            var norm = new AnalystNormalizeCSV {Script = Script};
            Analyst.CurrentQuantTask = norm;
            norm.Report = new AnalystReportBridge(Analyst);

            bool headers = Script.ExpectInputHeaders(sourceID);
            norm.Analyze(sourceFile, headers, format, Analyst);
            norm.ProduceOutputHeaders = true;
            norm.Normalize(targetFile);
            Analyst.CurrentQuantTask = null;
            return norm.ShouldStop();
        }
    }
}
