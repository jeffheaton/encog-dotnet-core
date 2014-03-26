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
using ConsoleExamples.Examples;
using Encog.Cloud.Indicator;
using Encog.Cloud.Indicator.Basic;
using Encog.Cloud.Indicator.Server;

namespace Encog.Examples.Indicator.ImportData
{
    public class ImportIndicatorData : IIndicatorConnectionListener, IExample
    {
        public const int Port = 5128;

        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (ImportIndicatorData),
                    "indicator-download",
                    "Download data from Ninjatrader.",
                    "Uses the Encog Framework indicator to download data from Ninjatrader.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            this.app = app;

            Console.WriteLine(@"Waiting for connections on port " + Port);

            var ind = new DownloadIndicatorFactory(new FileInfo("d:\\ninja.csv"));
            ind.RequestData("HIGH[5]");
            ind.RequestData("LOW[1]");
            ind.RequestData("OPEN[1]");
            ind.RequestData("CLOSE[1]");
            ind.RequestData("VOL[1]");
            ind.RequestData("MACD(12,26,9).Avg[1]");

            var server = new IndicatorServer();
            server.AddListener(this);
            server.AddIndicatorFactory(ind);
            server.Start();
            server.WaitForIndicatorCompletion();
        }

        #endregion

        #region IIndicatorConnectionListener Members

        public void NotifyConnections(IndicatorLink link, bool hasOpened)
        {
            if (hasOpened)
            {
                Console.WriteLine(@"Connection from " + link.ClientSocket.RemoteEndPoint + @" established.");
            }
            else
            {
                Console.WriteLine(@"Connection from " + link.ClientSocket.RemoteEndPoint + @" terminated.");
            }
        }

        #endregion
    }
}
