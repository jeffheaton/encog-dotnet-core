using System;
using System.IO;
using Encog.ML.Data;
using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;
using Encog.Neural.Networks;
using Encog.Persist;
using Encog.Util;
using Encog.Util.File;

namespace Encog.Examples.CSVMarketExample
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
            IMarketLoader loader = new CSVLoader();
            loader.GetFile(newfileLoad);

            var result = new MarketMLDataSet(loader,
                                             Config.INPUT_WINDOW, Config.PREDICT_WINDOW);
          //  var desc = new MarketDataDescription(Config.TICKER,
                                              //   MarketDataType.Close, true, true);

            var desc = new MarketDataDescription(Config.TICKER,
                                     MarketDataType.Trade, true, true);
            result.AddDescription(desc);

            var end = DateTime.Now; // end today
            var begin = new DateTime(end.Ticks); // begin 30 days ago
            begin = begin.AddDays(-150);

            result.Load(begin, end);
            result.Generate();

            return result;
        }

        public static void Evaluate(FileInfo dataDir,string filename)
        {
            FileInfo file = FileUtil.CombinePath(dataDir, Config.NETWORK_FILE);

            if (!file.Exists)
            {
                Console.WriteLine(@"Can't read file: " + file);
                return;
            }

            var network = (BasicNetwork) EncogDirectoryPersistence.LoadObject(file);

            MarketMLDataSet data = GrabData(filename);

            int count = 0;
            int correct = 0;
            foreach (IMLDataPair pair in data)
            {
                IMLData input = pair.Input;
                IMLData actualData = pair.Ideal;
                IMLData predictData = network.Compute(input);

                double actual = actualData[0];
                double predict = predictData[0];
                double diff = Math.Abs(predict - actual);

                Direction actualDirection = DetermineDirection(actual);
                Direction predictDirection = DetermineDirection(predict);

                if (actualDirection == predictDirection)
                    correct++;

                count++;


                Console.WriteLine(@"Day " + count + @":actual="
                                  + Format.FormatDouble(actual, 4) + @"(" + actualDirection + @")"
                                  + @",predict=" + Format.FormatDouble(predict, 4) + @"("
                                  + predictDirection + @")" + @",diff=" + diff);
            }
            double percent = correct/(double) count;
            Console.WriteLine(@"Direction correct:" + correct + @"/" + count);
            Console.WriteLine(@"Directional Accuracy:"
                              + Format.FormatPercent(percent));
        }
    }
}