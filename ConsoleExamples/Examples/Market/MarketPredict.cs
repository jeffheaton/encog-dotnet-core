using System;
using System.IO;
using ConsoleExamples.Examples;

namespace Encog.Examples.Market
{
    public class MarketPredict : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(MarketPredict),
                    "market",
                    "Simple Market Prediction",
                    "Use EOD data to predict direction of a stock.");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            if (app.Args.Length < 1)
            {
                Console.WriteLine(@"MarketPredict [data dir] [generate/train/prune/evaluate]");
            }
            else
            {
                var dataDir = new FileInfo(app.Args[0]);
                if (String.Compare(app.Args[1], "generate", true) == 0)
                {
                    MarketBuildTraining.Generate(dataDir);
                }
                else if (String.Compare(app.Args[1], "train", true) == 0)
                {
                    MarketTrain.Train(dataDir);
                }
                else if (String.Compare(app.Args[1], "evaluate", true) == 0)
                {
                    MarketEvaluate.Evaluate(dataDir);
                }
                else if (String.Compare(app.Args[1], "prune", true) == 0)
                {
                    {
                        MarketPrune.Incremental(dataDir);
                    }
                }
            }
        }
    }
}