using System;
using System.IO;
using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Util.File;
using Encog.Util.Simple;

namespace Encog.Examples.RangeCalculators
{
    public class MarketBuildTraining
    {
        public static void Generate(string fileName)
        {

          
            FileInfo dataDir = new FileInfo(@Environment.CurrentDirectory);
            IMarketLoader loader = new CSVLoader();
            var market = new MarketMLDataSet(loader, RangeConfig.INPUT_WINDOW, RangeConfig.PREDICT_WINDOW);
          //  var desc = new MarketDataDescription(Config.TICKER, MarketDataType.Close, true, true);

            var desc = new MarketDataDescription(RangeConfig.TICKER, MarketDataType.Trade, true, true);
            market.AddDescription(desc);
            string currentDirectory =@"c:\";
            loader.GetFile(fileName);

            var end = DateTime.Now; // end today
            var begin = new DateTime(end.Ticks); // begin 30 days ago

            // Gather training data for the last 2 years, stopping 60 days short of today.
            // The 60 days will be used to evaluate prediction.
            begin = begin.AddDays(-200);
            end = begin.AddDays(1);
           
            Console.WriteLine("You are loading date from:" + begin.ToShortDateString() + " To :" + end.ToShortDateString());

            market.Load(begin, end);
            market.Generate();
            EncogUtility.SaveEGB(FileUtil.CombinePath(dataDir, RangeConfig.TRAINING_FILE), market);

            // create a network
            BasicNetwork network = EncogUtility.SimpleFeedForward(
                market.InputSize,
                RangeConfig.HIDDEN1_COUNT,
                RangeConfig.HIDDEN2_COUNT,
                market.IdealSize,
                true);

            // save the network and the training
            EncogDirectoryPersistence.SaveObject(FileUtil.CombinePath(dataDir, RangeConfig.NETWORK_FILE), network);
        }
    }
}