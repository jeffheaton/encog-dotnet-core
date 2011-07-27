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

namespace Encog.Examples.CSVMarketExample
{
    public class CSVMarketExample : IExample
    {
        public const int CHAR_WIDTH = 5;
        public const int CHAR_HEIGHT = 7;



        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(CSVMarketExample),
                    "CSVPredict",
                    "CSVPredict",
                    "Load CSV and make market predictions in the console.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            //placed a try catch in case something bugs.
            try
            {
                //lets check the lenght of the input from the console.
                if (app.Args.Length < 1)
                {
                    Console.WriteLine(@"MarketPredict [generate/train/prune/evaluate] [PathToFile]");
                    Console.WriteLine(@"e.g csvMarketPredict [generate/train/prune/evaluate] c:\\EURUSD.csv");
                }

           //looks like we are fine.
                else
                {

                    //save the files in the directory where the consoleexample.exe is located.
                    FileInfo dataDir = new FileInfo(@"c:\");

                    //we generate the network , by calling the CSVloader.
                    if (String.Compare(app.Args[0], "generate", true) == 0)
                    {
                        Console.WriteLine("Generating your network with file:" + app.Args[1]);
                        MarketBuildTraining.Generate(app.Args[1]);
                    }

                    //train the network here.
                    else if (String.Compare(app.Args[0], "train", true) == 0)
                    {
                        MarketTrain.Train(dataDir);
                    }
                    //Evaluate the network that was built and trained.
                    else if (String.Compare(app.Args[0], "evaluate", true) == 0)
                    {
                        MarketEvaluate.Evaluate(dataDir,app.Args[1]);
                    }
                    //Lets prune the network.
                    else if (String.Compare(app.Args[0], "prune", true) == 0)
                    {

                        MarketPrune.Incremental(dataDir);

                    }
                    else
                    {
                        Console.WriteLine("Didnt understand the command you typed:" + app.Args[0]);

                    }
                }
            }

              
            catch (Exception ex)
            {

                Console.WriteLine("Error Message:"+ ex.Message);
                Console.WriteLine("Error Innerexception:" + ex.InnerException);
                Console.WriteLine("Error stacktrace:" + ex.StackTrace);
                Console.WriteLine("Error source:" + ex.Source);
            }
             

        }

        #endregion

       
    }
    
}