using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Cloud.Indicator.Basic;
using Encog.ML;
using Encog.Util.Arrayutil;
using Encog.Cloud.Indicator.Server;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util.CSV;
using Encog.Util.File;
using System.IO;

namespace Encog.Examples.Indicator.Ninja.Avg
{
    /// <summary>
    /// This is the actual indicator that will be called remotely from 
    /// NinjaTrader.
    /// </summary>
    public class MyInd : BasicIndicator
    {
        /// <summary>
        /// Holds the data as it is downloaded.
        /// </summary>
        private InstrumentHolder holder = new InstrumentHolder();

        /// <summary>
        /// The number of rows downloaded.
        /// </summary>
        private int rowsDownloaded;

        /// <summary>
        /// The path to store the data files.
        /// </summary>
        private string path;

        /// <summary>
        /// The machine learning method used to predict.
        /// </summary>
        private IMLRegression method;

        /// <summary>
        /// Used to normalize the difference between the two SMAs.
        /// </summary>
        private readonly NormalizedField fieldDifference;

        /// <summary>
        /// Used to normalize the pip profit/loss outcome.
        /// </summary>
        private readonly NormalizedField fieldOutcome;

        /// <summary>
        /// Construct the indicator. 
        /// </summary>
        /// <param name="theMethod">The machine learning method to use.</param>
        /// <param name="thePath">The path to use.</param>
        public MyInd(IMLRegression theMethod, string thePath)
            : base(theMethod != null)
        {
            this.method = theMethod;
            this.path = thePath;

            RequestData("CLOSE[1]");
            RequestData("SMA(10)[" + Config.INPUT_WINDOW + "]");
            RequestData("SMA(25)[" + Config.INPUT_WINDOW + "]");

            this.fieldDifference = new NormalizedField(NormalizationAction.Normalize, "diff", Config.DIFF_RANGE, -Config.DIFF_RANGE, 1, -1);
            this.fieldOutcome = new NormalizedField(NormalizationAction.Normalize, "out", Config.PIP_RANGE, -Config.PIP_RANGE, 1, -1);
        }

        /// <summary>
        /// Called to notify the indicator that a bar has been received. 
        /// </summary>
        /// <param name="packet">The packet received.</param>
        public override void NotifyPacket(IndicatorPacket packet)
        {
            String security = packet.Args[1];
            long when = long.Parse(packet.Args[0]);
            String key = security.ToLower();

            if (this.method == null)
            {
                if (holder.Record(when, 2, packet.Args))
                {
                    this.rowsDownloaded++;
                }
            }
            else
            {
                BasicMLData input = new BasicMLData(Config.PREDICT_WINDOW);

                int fastIndex = 2;
                int slowIndex = fastIndex + Config.INPUT_WINDOW;

                for (int i = 0; i < 3; i++)
                {
                    double fast = CSVFormat.EgFormat.Parse(packet.Args[fastIndex + i]);
                    double slow = CSVFormat.EgFormat.Parse(packet.Args[slowIndex + i]);
                    double diff = this.fieldDifference.Normalize((fast - slow) / Config.PIP_SIZE);
                    input[i] = this.fieldDifference.Normalize(diff);
                }

                IMLData result = this.method.Compute(input);

                double d = result[0];
                d = this.fieldOutcome.DeNormalize(d);

                String[] args = { 
					"?",	// line 1
					"?",	// line 2
					"?",	// line 3
					CSVFormat.EgFormat.Format(d,EncogFramework.DefaultPrecision), // bar 1
					"?", // bar 2
					"?", // bar 3
					"?", // arrow 1
					"?"}; // arrow 2

                this.Link.WritePacket(IndicatorLink.PACKET_IND, args);
            }
        }

        /// <summary>
        /// Determine the next file to process. 
        /// </summary>
        /// <returns>The next file.</returns>
        public string NextFile()
        {
            int mx = -1;
            string[] list = System.IO.Directory.GetFiles(path);

            foreach (string file in list)
            {
                String fn = FileUtil.GetFileName(new FileInfo(file));
                if (fn.StartsWith("collected") && fn.EndsWith(".csv"))
                {
                    int idx = fn.IndexOf(".csv");
                    String str = fn.Substring(9, idx);
                    int n = int.Parse(str);
                    mx = Math.Max(n, mx);
                }
            }

            return FileUtil.CombinePath(new FileInfo(path), "collected" + (mx + 1) + ".csv").ToString();
        }

        /// <summary>
        /// Write the files that were collected.
        /// </summary>
        public void WriteCollectedFile()
        {
            string targetFile = NextFile();

            using (StreamWriter outfile = new StreamWriter(targetFile))
            {
                // output header
                outfile.Write("\"WHEN\"");
                int index = 0;
                foreach (String str in this.DataRequested)
                {
                    String str2;

                    // strip off [ if needed
                    int ix = str.IndexOf('[');
                    if (ix != -1)
                    {
                        str2 = str.Substring(0, ix).Trim();
                    }
                    else
                    {
                        str2 = str;
                    }

                    int c = DataCount[index++];
                    if (c <= 1)
                    {
                        outfile.Write(",\"" + str2 + "\"");
                    }
                    else
                    {
                        for (int i = 0; i < c; i++)
                        {
                            outfile.Write(",\"" + str2 + "-b" + i + "\"");
                        }
                    }
                }
                outfile.WriteLine();

                // output data

                foreach (long key in holder.Sorted)
                {
                    String str = holder.Data[key];
                    outfile.WriteLine(key + "," + str);
                }
            }
        }

        /// <summary>
        /// Notify on termination, write the collected file.
        /// </summary>
        public override void NotifyTermination()
        {
            if (this.method == null)
            {
                WriteCollectedFile();
            }
        }

        /// <summary>
        /// The number of rows downloaded.
        /// </summary>
        public int RowsDownloaded
        {
            get
            {
                return rowsDownloaded;
            }
        }
    }
}
