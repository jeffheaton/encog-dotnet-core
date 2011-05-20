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
    /// This command is used to randomize the lines in a CSV file.
    /// </summary>
    ///
    public class CmdCluster : Cmd
    {
        /// <summary>
        /// The default number of iterations.
        /// </summary>
        ///
        public const int DefaultIterations = 100;

        /// <summary>
        /// The name of this command.
        /// </summary>
        ///
        public const String CommandName = "CLUSTER";

        /// <summary>
        /// Construct the cluster command.
        /// </summary>
        ///
        /// <param name="analyst">The analyst object to use.</param>
        public CmdCluster(EncogAnalyst analyst) : base(analyst)
        {
        }

        /// <inheritdoc/>
        public override String Name
        {
            get { return CommandName; }
        }

        /// <inheritdoc/>
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

            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "Beginning cluster");
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "source file:" + sourceID);
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "target file:" + targetID);
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "clusters:" + clusters);

            FileInfo sourceFile = Script.ResolveFilename(sourceID);
            FileInfo targetFile = Script.ResolveFilename(targetID);

            // get formats
            CSVFormat inputFormat = Script
                .DetermineInputFormat(sourceID);
            CSVFormat outputFormat = Script.DetermineOutputFormat();

            // mark generated
            Script.MarkGenerated(targetID);

            // prepare to normalize
            var cluster = new AnalystClusterCSV {Script = Script};
            Analyst.CurrentQuantTask = cluster;
            cluster.Report = new AnalystReportBridge(Analyst);
            bool headers = Script.ExpectInputHeaders(sourceID);
            cluster.Analyze(Analyst, sourceFile, headers, inputFormat);
            cluster.OutputFormat = outputFormat;
            cluster.Process(targetFile, clusters, Analyst, DefaultIterations);
            Analyst.CurrentQuantTask = null;
            return cluster.ShouldStop();
        }
    }
}