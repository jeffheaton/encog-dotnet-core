using System;
using ConsoleExamples.Examples;
using Encog.Cloud.Indicator;
using Encog.Cloud.Indicator.Server;

namespace Encog.Examples.Indicator.CustomInd
{
    public class RemoteEMA : IExample, IIndicatorConnectionListener
    {
        public const int Port = 5128;

        private IExampleInterface _app;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (RemoteEMA),
                    "indicator-ema",
                    "Provide a EMA indicator.",
                    "Provide a EMA indicator to the Encog Framework Indicator.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            _app = app;
            Console.WriteLine(@"Waiting for connections on port " + Port);

            var server = new IndicatorServer();
            server.AddListener(this);

            server.AddIndicatorFactory(new MyFactory());

            server.Start();
            server.WaitForShutdown();
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

        #region Nested type: MyFactory

        public class MyFactory : IIndicatorFactory
        {
            #region IIndicatorFactory Members

            public String Name
            {
                get { return "EMA"; }
            }

            public IIndicatorListener Create()
            {
                return new EMA();
            }

            #endregion
        }

        #endregion
    }
}