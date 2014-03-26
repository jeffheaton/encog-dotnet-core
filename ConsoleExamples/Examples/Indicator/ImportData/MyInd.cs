//
// Encog(tm) Core v3.2 - .Net Version
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
using Encog.Cloud.Indicator.Basic;
using Encog.Cloud.Indicator.Server;
using Encog.Examples.Indicator.Avg;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util.Arrayutil;
using Encog.Util.CSV;
using Encog.Util.File;
using Directory = System.IO.Directory;

namespace Encog.Examples.Indicator.ImportData
{
    /// <summary>
    /// This is the actual indicator that will be called remotely from 
    /// NinjaTrader.
    /// </summary>
    public class MyInd : BasicIndicator
    {
        /// <summary>
        /// Used to normalize the difference between the two SMAs.
        /// </summary>
        private readonly NormalizedField _fieldDifference;

        /// <summary>
        /// Used to normalize the pip profit/loss outcome.
        /// </summary>
        private readonly NormalizedField _fieldOutcome;

        /// <summary>
        /// Holds the data as it is downloaded.
        /// </summary>
        private readonly InstrumentHolder _holder = new InstrumentHolder();

        /// <summary>
        /// The machine learning method used to predict.
        /// </summary>
        private readonly IMLRegression _method;

        /// <summary>
        /// The path to store the data files.
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// The number of rows downloaded.
        /// </summary>
        private int _rowsDownloaded;

        /// <summary>
        /// Construct the indicator. 
        /// </summary>
        /// <param name="theMethod">The machine learning method to use.</param>
        /// <param name="thePath">The path to use.</param>
        public MyInd(IMLRegression theMethod, string thePath)
            : base(theMethod != null)
        {
            _method = theMethod;
            _path = thePath;

            RequestData("CLOSE[1]");
            RequestData("SMA(10)[" + Config.InputWindow + "]");
            RequestData("SMA(25)[" + Config.InputWindow + "]");

            _fieldDifference = new NormalizedField(NormalizationAction.Normalize, "diff", Config.DiffRange,
                                                  -Config.DiffRange, 1, -1);
            _fieldOutcome = new NormalizedField(NormalizationAction.Normalize, "out", Config.PipRange, -Config.PipRange,
                                               1, -1);
        }

        /// <summary>
        /// The number of rows downloaded.
        /// </summary>
        public int RowsDownloaded
        {
            get { return _rowsDownloaded; }
        }

        /// <summary>
        /// Called to notify the indicator that a bar has been received. 
        /// </summary>
        /// <param name="packet">The packet received.</param>
        public override void NotifyPacket(IndicatorPacket packet)
        {
            long when = long.Parse(packet.Args[0]);         

            if (_method == null)
            {
                if (_holder.Record(when, 2, packet.Args))
                {
                    _rowsDownloaded++;
                }
            }
            else
            {
                var input = new BasicMLData(Config.PredictWindow);

                const int fastIndex = 2;
                const int slowIndex = fastIndex + Config.InputWindow;

                for (int i = 0; i < 3; i++)
                {
                    double fast = CSVFormat.EgFormat.Parse(packet.Args[fastIndex + i]);
                    double slow = CSVFormat.EgFormat.Parse(packet.Args[slowIndex + i]);
                    double diff = _fieldDifference.Normalize((fast - slow)/Config.PipSize);
                    input[i] = _fieldDifference.Normalize(diff);
                }

                IMLData result = _method.Compute(input);

                double d = result[0];
                d = _fieldOutcome.DeNormalize(d);

                String[] args = {
                                    "?", // line 1
                                    "?", // line 2
                                    CSVFormat.EgFormat.Format(d, EncogFramework.DefaultPrecision), // bar 1
                                }; // arrow 2

                Link.WritePacket(IndicatorLink.PacketInd, args);
            }
        }

        /// <summary>
        /// Determine the next file to process. 
        /// </summary>
        /// <returns>The next file.</returns>
        public string NextFile()
        {
            int mx = -1;
            string[] list = Directory.GetFiles(_path);

            foreach (string file in list)
            {
                var fn = new FileInfo(file);
                if (fn.Name.StartsWith("collected") && fn.Name.EndsWith(".csv"))
                {
                    int idx = fn.Name.IndexOf(".csv");
                    String str = fn.Name.Substring(9, idx - 9);
                    int n = int.Parse(str);
                    mx = Math.Max(n, mx);
                }
            }

            return FileUtil.CombinePath(new FileInfo(_path), "collected" + (mx + 1) + ".csv").ToString();
        }

        /// <summary>
        /// Write the files that were collected.
        /// </summary>
        public void WriteCollectedFile()
        {
            string targetFile = NextFile();

            using (var outfile = new StreamWriter(targetFile))
            {
                // output header
                outfile.Write("\"WHEN\"");
                int index = 0;
                foreach (String str in DataRequested)
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

                foreach (long key in _holder.Sorted)
                {
                    String str = _holder.Data[key];
                    outfile.WriteLine(key + "," + str);
                }
            }
        }

        /// <summary>
        /// Notify on termination, write the collected file.
        /// </summary>
        public override void NotifyTermination()
        {
            if (_method == null)
            {
                WriteCollectedFile();
            }
        }
    }
}
