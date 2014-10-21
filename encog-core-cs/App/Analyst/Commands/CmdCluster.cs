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
using Encog.App.Analyst.CSV;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Util;
using Encog.Util.CSV;
using Encog.Util.Logging;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    ///     This command is used to randomize the lines in a CSV file.
    /// </summary>
    public class CmdCluster : Cmd
    {
        /// <summary>
        ///     The default number of iterations.
        /// </summary>
        public const int DefaultIterations = 100;

        /// <summary>
        ///     The name of this command.
        /// </summary>
        public const String CommandName = "CLUSTER";

        /// <summary>
        ///     Construct the cluster command.
        /// </summary>
        /// <param name="analyst">The analyst object to use.</param>
        public CmdCluster(EncogAnalyst analyst) : base(analyst)
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
                ScriptProperties.ClusterConfigSourceFile);
            String targetID = Prop.GetPropertyString(
                ScriptProperties.ClusterConfigTargetFile);
            int clusters = Prop.GetPropertyInt(
                ScriptProperties.ClusterConfigClusters);
            Prop.GetPropertyString(ScriptProperties.ClusterConfigType);

            EncogLogging.Log(EncogLogging.LevelDebug, "Beginning cluster");
            EncogLogging.Log(EncogLogging.LevelDebug, "source file:" + sourceID);
            EncogLogging.Log(EncogLogging.LevelDebug, "target file:" + targetID);
            EncogLogging.Log(EncogLogging.LevelDebug, "clusters:" + clusters);

            FileInfo sourceFile = Script.ResolveFilename(sourceID);
            FileInfo targetFile = Script.ResolveFilename(targetID);

            // get formats
            CSVFormat format = Script.DetermineFormat();

            // mark generated
            Script.MarkGenerated(targetID);

            // prepare to normalize
            var cluster = new AnalystClusterCSV {Script = Script};
            Analyst.CurrentQuantTask = cluster;
            cluster.Report = new AnalystReportBridge(Analyst);
            bool headers = Script.ExpectInputHeaders(sourceID);
            cluster.Analyze(Analyst, sourceFile, headers, format);
            cluster.Process(targetFile, clusters, Analyst, DefaultIterations);
            Analyst.CurrentQuantTask = null;
            return cluster.ShouldStop();
        }
    }
}
