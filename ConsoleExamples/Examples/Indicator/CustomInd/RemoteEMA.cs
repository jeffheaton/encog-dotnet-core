using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Cloud.Indicator;
using Encog.Cloud.Indicator.Server;

namespace Encog.Examples.Indicator.CustomInd
{
    public class RemoteEMA : IExample, IIndicatorConnectionListener
    {
        public const int PORT = 5128;

        private IExampleInterface app;

        public class MyFactory : IIndicatorFactory
        {
            public String Name
            {
                get
                {
                    return "EMA";
                }
            }

            public IIndicatorListener Create()
            {
                return new EMA();
            }
        }

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(RemoteEMA),
                    "indicator-ema",
                    "Provide a EMA indicator.",
                    "Provide a EMA indicator to the Encog Framework Indicator.");
                return info;
            }
        }


        public void Execute(IExampleInterface app)
        {
            this.app = app;
            Console.WriteLine("Waiting for connections on port " + PORT);

            IndicatorServer server = new IndicatorServer();
            server.AddListener(this);

            server.AddIndicatorFactory(new MyFactory());

            server.Start();
            server.WaitForShutdown();


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
