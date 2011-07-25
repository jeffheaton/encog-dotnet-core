//
// Encog(tm) Console Examples v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.MathUtil.Randomize;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Simple;
using Encog.Neural.Pattern;
using Encog.Util.CSV;
using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;
using Encog.Examples.Market;
using Encog.Util.Simple;
using Encog.Util.File;
using System.IO;
using Encog.Persist;

namespace Encog.Examples.Adaline
{
    public class MyTests : IExample
    {
        public const int CHAR_WIDTH = 5;
        public const int CHAR_HEIGHT = 7;



        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(MyTests),
                    "Tests",
                    "Testing",
                    "Just a small console app to test some stuff..");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {


            string dataDir = "c:\\EncogOutput";

            Array a = Enum.GetValues(typeof(MarketDataType));
            string[] ab = new string[a.Length];

            ab = Enum.GetNames(typeof(MarketDataType));

            foreach (string item in ab)
            {
                Console.WriteLine("MarketType:" + item);
            }


            IMarketLoader loader = new CSVLoader();

            var market = new MarketMLDataSet(loader,
                                          Config.INPUT_WINDOW, Config.PREDICT_WINDOW);
            var desc = new MarketDataDescription(
                Config.TICKER, MarketDataType.Close, true, true);
            market.AddDescription(desc);

            var end = DateTime.Now; // end today
            var begin = new DateTime(end.Ticks); // begin 30 days ago

            // Gather training data for the last 2 years, stopping 60 days short of today.
            // The 60 days will be used to evaluate prediction.
            begin = begin.AddDays(-60);
            end = end.AddDays(-60);
            begin = begin.AddYears(-2);

            market.Load(begin, end);
            market.Generate();
            EncogUtility.SaveEGB(FileUtil.CombinePath(new FileInfo(dataDir), Config.TRAINING_FILE), market);
            // create a network
            BasicNetwork network = EncogUtility.SimpleFeedForward(
                market.InputSize,
                Config.HIDDEN1_COUNT,
                Config.HIDDEN2_COUNT,
                market.IdealSize,
                true);

            // save the network and the training
            EncogDirectoryPersistence.SaveObject(FileUtil.CombinePath(new FileInfo(dataDir), Config.NETWORK_FILE), network);


       
            // train the neural network
            EncogUtility.TrainConsole(network, market,1);

            Console.WriteLine(@"Final Error: " + network.CalculateError(market));
            Console.WriteLine(@"Training complete, saving network.");
            EncogDirectoryPersistence.SaveObject(new FileInfo(Config.NETWORK_FILE), network);
            Console.WriteLine(@"Network saved.");

            EncogFramework.Instance.Shutdown();
        }

        #endregion

       
    }
}