using System;
using System.IO;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;
using Encog.ML.Data.Temporal;
using Encog.Neural.Networks;
using Encog.Neural.Pattern;
using Encog.Util;
using Encog.Util.File;

namespace Encog.Examples.RangeAndPredictions
{
    public class MarketEvaluate
    {
        #region Direction enum

        public enum Direction
        {
            Up,
            Down
        } ;

        #endregion

        public static Direction DetermineDirection(double d)
        {
            return d < 0 ? Direction.Down : Direction.Up;
        }
       
        public static MarketMLDataSet GrabData(string newfileLoad)
        {
            FileInfo dataDir = new FileInfo(@Environment.CurrentDirectory);
            IMarketLoader loader = new RangeMaker();
            loader.GetFile(newfileLoad);

            var market = new MarketMLDataSet(loader, Examples.RangeCalculators.RangeConfig.INPUT_WINDOW, Examples.RangeCalculators.RangeConfig.PREDICT_WINDOW);


            //var descClose = new MarketDataDescription(RangeConfig.TICKER, MarketDataType.Close, true, false);
            //market.AddDescription(descClose);
            //var descOpen = new MarketDataDescription(RangeConfig.TICKER, MarketDataType.Open, true, false);
            //market.AddDescription(descOpen);
            //var descHigh = new MarketDataDescription(RangeConfig.TICKER, MarketDataType.High, true, false);
            //market.AddDescription(descHigh);
            //var descLow = new MarketDataDescription(RangeConfig.TICKER, MarketDataType.Low, true, false);
            //market.AddDescription(descLow);
            //var ranges = new MarketDataDescription(RangeConfig.TICKER, MarketDataType.RangeOpenClose,
            //                                       TemporalDataDescription.Type.Raw, true, false);
           // market.AddDescription(ranges);
            var RangeopenClose = new MarketDataDescription(Examples.RangeCalculators.RangeConfig.TICKER, MarketDataType.RangeHighLow,
                                                           TemporalDataDescription.Type.Raw, true, true);
            market.AddDescription(RangeopenClose);
            var end = DateTime.Now; // end today
            var begin = new DateTime(end.Ticks); // begin 30 days ago
            // Gather training data for the last 2 years, stopping 60 days short of today.
            // The 60 days will be used to evaluate prediction.
            begin = begin.AddDays(-450);
            end = begin.AddDays(60);

            Console.WriteLine(@"You are loading date from:" + begin.ToShortDateString() + @" To :" +
                              end.ToShortDateString());

            market.Load(begin, end);
            market.Generate();
            return market;

        }
        
        public static double DeterminePrice (double percentage , double currentPrice)
        {
            return (percentage*currentPrice) + currentPrice;
        }
        
        public static IMLMethod CreateFeedforwardNetwork(int inputneurons, int outputsneurons, int hiddenNeurons, int hidden2)
        {
            // construct a feedforward type network
            FeedForwardPattern pattern = new FeedForwardPattern();
            pattern.ActivationFunction = new ActivationSigmoid();
            pattern.InputNeurons = inputneurons;
            pattern.AddHiddenLayer(hiddenNeurons);
            pattern.AddHiddenLayer(hidden2);
            pattern.OutputNeurons = outputsneurons;
            return pattern.Generate();
        }


        //public static void Evaluate(FileInfo dataDir,string filename)
        //{
        //    FileInfo file = FileUtil.CombinePath(dataDir, Examples.RangeCalculators.RangeConfig.NETWORK_FILE);

        //    if (!file.Exists)
        //    {
        //        Console.WriteLine(@"Can't read file: " + file);
        //        return;
        //    }

        //    //var network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(file);

        //    MarketMLDataSet data = GrabData(filename);
        //    var network = (BasicNetwork)  MarketBuildTraining.NetworkBuilders.CreateFeedforwardNetwork(data.InputNeuronCount, data.IdealSize,
        //                                        Examples.RangeCalculators.RangeConfig.HIDDEN1_COUNT, Examples.RangeCalculators.RangeConfig.HIDDEN2_COUNT);


        //    int count = 0;
        //    int correct = 0;
        //    foreach (IMLDataPair pair in data)
        //    {
        //        IMLData input = pair.Input;
        //        IMLData actualData = pair.Ideal;
        //        IMLData predictData = network.Compute(input);

        //        double actual = actualData[0];
        //        double predict = predictData[0];
        //        double diff = Math.Abs(predict - actual);

        //        Direction actualDirection = DetermineDirection(actual);
        //        Direction predictDirection = DetermineDirection(predict);

        //        if (actualDirection == predictDirection)
        //            correct++;

        //        count++;


        //        Console.WriteLine(@"Day " + count + @":actual="
        //                          + Format.FormatDouble(actual, 4) + @"(" + actualDirection + @")"
        //                          + @",predict=" + Format.FormatDouble(predict, 4) + @"("
        //                          + predictDirection + @")" + @",diff=" + diff);
        //    }
        //    double percent = correct/(double) count;
        //    Console.WriteLine(@"Direction correct:" + correct + @"/" + count);
        //    Console.WriteLine(@"Directional Accuracy:"
        //                      + Format.FormatPercent(percent));
        //}
    }
}