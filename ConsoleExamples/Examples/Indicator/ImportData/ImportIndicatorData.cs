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