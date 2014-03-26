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
