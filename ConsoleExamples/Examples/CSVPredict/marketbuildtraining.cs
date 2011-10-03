using System;
using System.IO;
using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Util.File;
using Encog.Util.Simple;

namespace Encog.Examples.CSVMarketExample
{
    public class MarketBuildTraining
    {
        public static void Generate(string fileName)
        {

          
            FileInfo dataDir = new FileInfo(@Environment.CurrentDirectory);
            //Lets use the CSVFinal..(and not the CSV Form loader).
            IMarketLoader loader = new CSVFinal();
            loader.GetFile(fileName);
            var market = new MarketMLDataSet(loader,(ulong)Config.INPUT_WINDOW, (ulong)Config.PREDICT_WINDOW);
          //  var desc = new MarketDataDescription(Config.TICKER, MarketDataType.Close, true, true);

            var desc = new MarketDataDescription(Config.TICKER, MarketDataType.Close, true, true);
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
            EncogUtility.SaveEGB(FileUtil.CombinePath(dataDir, Config.TRAINING_FILE), market);

            // create a network
            BasicNetwork network = EncogUtility.SimpleFeedForward(
                market.InputSize,
                Config.HIDDEN1_COUNT,
                Config.HIDDEN2_COUNT,
                market.IdealSize,
                true);

            // save the network and the training
            EncogDirectoryPersistence.SaveObject(FileUtil.CombinePath(dataDir,Config.NETWORK_FILE), network);
        }
    }
}