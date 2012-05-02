using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Cloud.Indicator;
using Encog.Cloud.Indicator.Basic;
using Encog.Cloud.Indicator.Server;
using System.IO;
using ConsoleExamples.Examples;

namespace Encog.Examples.Indicator.Ninja
{
    public class ImportNinjaData : IIndicatorConnectionListener, IExample
    {
        public const int PORT = 5128;

        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(ImportNinjaData),
                    "indicator-download",
                    "Download data from Ninjatrader.",
                    "Uses the Encog Framework indicator to download data from Ninjatrader.");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            this.app = app;

            Console.WriteLine("Waiting for connections on port " + PORT);

            DownloadIndicatorFactory ind = new DownloadIndicatorFactory(new FileInfo("d:\\ninja.csv"));
            ind.RequestData("HIGH[5]");
            ind.RequestData("LOW[1]");
            ind.RequestData("OPEN[1]");
            ind.RequestData("CLOSE[1]");
            ind.RequestData("VOL[1]");
            ind.RequestData("MACD(12,26,9).Avg[1]");

            IndicatorServer server = new IndicatorServer();
            server.AddListener(this);
            server.AddIndicatorFactory(ind);
            server.Start();
            server.WaitForIndicatorCompletion();
        }

        public void NotifyConnections(IndicatorLink link, bool hasOpened)
        {
            if (hasOpened)
            {
                Console.WriteLine("Connection from " + link.ClientSocket.RemoteEndPoint.ToString() + " established.");
            }
            else if (!hasOpened)
            {
                Console.WriteLine("Connection from " + link.ClientSocket.RemoteEndPoint.ToString() + " terminated.");
            }

        }
    }
}
