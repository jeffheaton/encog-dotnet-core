using System;
using System.IO;
using ConsoleExamples.Examples;

namespace Encog.Examples.SuperPredict
{

    public class SuperPredict : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (SuperPredict),
                    "Super",
                    "Super Predictor Prediction",
                    "Predicts stuff with a bench of inputs.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            if (app.Args.Length < 1)
            {
                Console.WriteLine(@"Super predictors with a bunch of inputs. [generate/train/prune/evaluate] PathToFile");
                Console.WriteLine(@"e.g Super [generate/train/prune/evaluate] c:\\EURUSD.csv");
                Console.WriteLine(@"Super train Directory");
            }
            else
            {
                FileInfo dataDir = new FileInfo(Environment.CurrentDirectory);
                if (String.Compare(app.Args[0], "generate", true) == 0)
                {
                    MarketBuildTraining.Generate();
                }
                else if (String.Compare(app.Args[0], "train", true) == 0)
                {
                    Examples.RangeCalculators.MarketTrain.Train(dataDir);
                }
                else if (String.Compare(app.Args[0], "evaluate", true) == 0)
                {
                    //MarketEvaluate.Evaluate(dataDir,app.Args[1]);
                }
                else if (String.Compare(app.Args[0], "prune", true) == 0)
                {
                    {
                        Examples.RangeCalculators.MarketPrune.Incremental(dataDir);
                    }
                }
            }
        }

        #endregion
    }
}