using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;
using Encog.Util.File;
using Encog.Util.Simple;
using CSVanalyze.Properties;
using Encog.Util.Time;

namespace CSVanalyze
{
    /// <summary>
    /// A quick loader class that uses the Market data sets and loads data from a csv for analysis with encog.
    /// </summary>
    public class Loader
    {
        #region loads data from a csv file.
        /// <summary>
        /// Grabs the data from a csv and returns and imldataset.
        /// you can give multiple inputs and one input.
        /// you can use the boolean save to save (or not) , the dataset to file.
        /// The predict item is a string (" Close ") , which will be used to detect which data you want to predict in the list of MarketdataItems.
        /// You must provide a valid CSV (See CSVMetaTrader).
        /// this version enabled a from and to date to be specified.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="TypesToLoad">The types to load.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="PredictItem">The predict item, this is the only item in the market Datatype that will be predicted..it's also used as input..</param>
        /// <param name="save">if set to <c>true</c> [save].</param>
        /// <returns></returns>
        public static IMLDataSet GrabData(string fileName, List<MarketDataType> TypesToLoad, DateTime from , DateTime to, string PredictItem, bool save)
        {


            if (File.Exists(fileName))
            {
                Console.WriteLine("Couldn't file the file");
                return null;
            }
            Settings mysettings = new Settings();
            FileInfo dataDir = new FileInfo(@Environment.CurrentDirectory);
            //Lets use the CSVFinal..(and not the CSV Form loader).
            IMarketLoader loader = new CSVMetaTrader();
            loader.GetFile(fileName);
            var market = new MarketMLDataSet(loader, (ulong) mysettings.InputSize * (ulong)TypesToLoad.Count, (ulong) mysettings.OutPutSize);
            //  var desc = new MarketDataDescription(Config.TICKER, MarketDataType.Close, true, true);
            
            foreach (MarketDataType marketDataType in TypesToLoad)
            {
                if (marketDataType.ToString().Equals(PredictItem))
                {
                    var description = new MarketDataDescription(new TickerSymbol(Settings.Default.Symbol), marketDataType, true, true);
                    market.AddDescription(description);
                }
                else
                {
                    var description = new MarketDataDescription(new TickerSymbol(Settings.Default.Symbol), marketDataType, true, false);
                    market.AddDescription(description);
                }
              
            }
            //Lets load the data , as we have prepared everything normally.
            loader.GetFile(fileName);
            var end = to; // end today
            var begin = from; // begin 30 days ago
            market.Load(begin, end);
            market.Generate();

            if (save)
                EncogUtility.SaveEGB(FileUtil.CombinePath(dataDir, Config.TRAINING_FILE), market);
            return market;
        }

        /// <summary>
        /// Grabs the data from the csv, this version uses the from and to date from the program settings.
        /// if you provide files made by meta trader nothing should have to be changed for this methods to work.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="TypesToLoad">The types to load.</param>
        /// <param name="PredictItem">The predict item.</param>
        /// <param name="save">if set to <c>true</c> [save].</param>
        /// <returns></returns>
        public static MarketMLDataSet GrabData(string fileName, List<MarketDataType> TypesToLoad, string PredictItem, bool save)
        {

            if (!File.Exists(fileName))
            {
                Console.WriteLine(@"Couldn't file the file: :" + fileName);
                return null;
            }
            Settings mysettings = new Settings();
            FileInfo dataDir = new FileInfo(@Environment.CurrentDirectory);

            IMarketLoader loader = new CSVMetaTrader();          
            var market = new MarketMLDataSet(loader, (ulong)mysettings.InputSize * (ulong)TypesToLoad.Count, (ulong)mysettings.OutPutSize);
            //  var desc = new MarketDataDescription(Config.TICKER, MarketDataType.Close, true, true);

            foreach (MarketDataType marketDataType in TypesToLoad)
            {
                if (marketDataType.ToString().Equals(PredictItem))
                {
                    var description = new MarketDataDescription(new TickerSymbol(Settings.Default.Symbol), marketDataType, true, true);
                    market.AddDescription(description);
                }
                else
                {
                    var description = new MarketDataDescription(new TickerSymbol(Settings.Default.Symbol), marketDataType, true, false);
                    market.AddDescription(description);
                }

            }
            //Lets load the data , as we have prepared everything normally.
            loader.GetFile(fileName);
            market.SequenceGrandularity = TimeUnit.Ticks;
         

            Console.WriteLine("You have :" + market.Descriptions.Count + " Data descriptions" + " Granularity is :" + market.SequenceGrandularity);
            var end = mysettings.EndDate; // end today
            var begin = mysettings.StartDate; // begin 30 days ago

            Console.WriteLine(" Start date is :" + begin + " end date:" + end);

            Console.WriteLine("Starting the loading data process.....");
            market.Load(begin, end);
            market.Generate();

            if (save)
                EncogUtility.SaveEGB(FileUtil.CombinePath(dataDir, Config.TRAINING_FILE), market);
            return market;
        }

        public static MarketMLDataSet GrabEvaluationData(string fileName, List<MarketDataType> TypesToLoad, string PredictItem, bool save, DateTime from , DateTime to)
        {

            if (!File.Exists(fileName))
            {
                Console.WriteLine(@"Couldn't file the file: :" + fileName);
                return null;
            }
            Settings mysettings = new Settings();
            FileInfo dataDir = new FileInfo(@Environment.CurrentDirectory);

            IMarketLoader loader = new CSVMetaTrader();
            var market = new MarketMLDataSet(loader, (ulong)mysettings.InputSize * (ulong)TypesToLoad.Count, (ulong)mysettings.OutPutSize);
            //  var desc = new MarketDataDescription(Config.TICKER, MarketDataType.Close, true, true);

            foreach (MarketDataType marketDataType in TypesToLoad)
            {
                if (marketDataType.ToString().Equals(PredictItem))
                {
                    var description = new MarketDataDescription(new TickerSymbol(Settings.Default.Symbol), marketDataType, true, true);
                    market.AddDescription(description);
                }
                else
                {
                    var description = new MarketDataDescription(new TickerSymbol(Settings.Default.Symbol), marketDataType, true, false);
                    market.AddDescription(description);
                }

            }
            //Lets load the data , as we have prepared everything normally.
            loader.GetFile(fileName);
            market.SequenceGrandularity = TimeUnit.Ticks;


            Console.WriteLine("You have :" + market.Descriptions.Count + " Data descriptions" + " Granularity is :" + market.SequenceGrandularity);
            var end = to; // end today
            var begin = from; // begin 30 days ago

            Console.WriteLine(" Start date is :" + begin + " end date:" + end);

            Console.WriteLine("Starting the loading data process.....");
            market.Load(begin, end);
            market.Generate();

            if (save)
                EncogUtility.SaveEGB(FileUtil.CombinePath(dataDir, Config.TRAINING_FILE), market);
            return market;
        }

        
        #endregion
    
    }
}
