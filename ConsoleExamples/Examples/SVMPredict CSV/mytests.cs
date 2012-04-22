//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
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
using Encog.Examples.SVMPredictCSV;

namespace Encog.Examples.SVMPredictCSV
{
    public class SVMPredictCSV : IExample
    {
        public const int CHAR_WIDTH = 5;
        public const int CHAR_HEIGHT = 7;



        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(SVMPredictCSV),
                    "SVMCSV",
                    "SVMCSV",
                    "Load CSV and make market predictions in the console via Super vector machines.");
                return info;
            }
        }

        #region IExample Members

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            if (app.Args.Length < 1)
            {
                Console.WriteLine(@"SVMcsv [generate/train/prune/evaluate] PathToFile");
                Console.WriteLine(@"e.g SVMcsv [generate/train/prune/evaluate] c:\\EURUSD.csv");
            }
            else
            {
                FileInfo dataDir = new FileInfo(Environment.CurrentDirectory);
                if (String.Compare(app.Args[0], "generate", true) == 0)
                {
                    MarketBuildTraining.Generate(app.Args[1]);
                }
                if (String.Compare(app.Args[0], "train", true) == 0)
                {
                    if (app.Args.Length > 0)
                    {
                        //We have enough arguments, lets test them.
                        if (File.Exists(app.Args[1]))
                        {
                            //the file exits lets build the training.
                            //create our basic ml dataset.
                            MarketPredict.TrainElmhanNetwork(ref app);
                        }
                    }
                }
                if (String.Compare(app.Args[0], "trainsvm", true) == 0)
                {
                    if (app.Args.Length > 0)
                    {
                        //We have enough arguments, lets test them.
                        if (File.Exists(app.Args[1]))
                        {
                            //the file exits lets build the training.
                            //create our basic ml dataset.
                            MarketPredict.TrainSVMNetwork(ref app);
                        }
                    }
                }
                else if (String.Compare(app.Args[0], "evaluate", true) == 0)
                {
                    MarketEvaluate.Evaluate(app.Args[1]);
                }
                else if (String.Compare(app.Args[0], "prune", true) == 0)
                {
                    {
                        MarketPrune.Incremental(dataDir);
                    }
                }

            }
        }
        #endregion

        #endregion

       
    }
    
}
