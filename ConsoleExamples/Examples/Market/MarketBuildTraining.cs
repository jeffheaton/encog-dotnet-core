using System;
using System.IO;
using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Util.File;
using Encog.Util.Simple;

namespace Encog.Examples.Market
{
    public class MarketBuildTraining
    {
        public static void Generate(FileInfo dataDir)
        {
            IMarketLoader loader = new YahooFinanceLoader();
            var market = new MarketMLDataSet(loader,
                                            (ulong) Config.INPUT_WINDOW, (ulong)Config.PREDICT_WINDOW);
            var desc = new MarketDataDescription(
                Config.TICKER, MarketDataType.AdjustedClose, true, true);
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
            EncogUtility.SaveEGB(FileUtil.CombinePath(dataDir, Config.TRAINING_FILE), market);

            // create a network
            BasicNetwork network = EncogUtility.SimpleFeedForward(
                market.InputSize,
                Config.HIDDEN1_COUNT,
                Config.HIDDEN2_COUNT,
                market.IdealSize,
                true);

            // save the network and the training
            EncogDirectoryPersistence.SaveObject(FileUtil.CombinePath(dataDir, Config.NETWORK_FILE), network);
        }
    }
}