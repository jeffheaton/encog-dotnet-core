using System;
using System.IO;
using ConsoleExamples.Examples;

namespace Encog.Examples.RangeCalculators
{

    public class RangeCalculators : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (RangeCalculators),
                    "csvrange",
                    "Simple Range Prediction",
                    "Use csv data to predict ranges of a stock or forex, futures instrument.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            if (app.Args.Length < 1)
            {
                Console.WriteLine(@"csvRange [generate/train/prune/evaluate] PathToFile");
                Console.WriteLine(@"e.g csvrange [generate/train/prune/evaluate] c:\\EURUSD.csv");
                Console.WriteLine(@"csvrange train Directory");
            }
            else
            {
                FileInfo dataDir = new FileInfo(Environment.CurrentDirectory);
                if (String.Compare(app.Args[0], "generate", true) == 0)
                {
                    MarketBuildTraining.Generate(app.Args[1]);
                }
                else if (String.Compare(app.Args[0], "train", true) == 0)
                {
                    MarketTrain.Train(dataDir);
                }
                else if (String.Compare(app.Args[0], "evaluate", true) == 0)
                {
                    MarketEvaluate.Evaluate(dataDir,app.Args[1]);
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
    }
}