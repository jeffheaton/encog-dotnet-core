using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Encog.ML.Data.Market;
using Encog.ML.Data.Market.Loader;
using Encog.App.Analyst;
using Encog.App.Analyst.Wizard;
using EncogExtensions.Normalization;
using System.Data;
using System.Linq;
using Encog.Util.Arrayutil;

namespace encog_extensions_test
{
    [TestClass]
    public class TestDataSetNormalization
    {
        public object ArrayUtil { get; private set; }

        [TestMethod]
        public void Normalize_Some_In_Memory_Data()
        {
            List<LoadedMarketData> MarketData = new List<LoadedMarketData>();
            MarketData.AddRange(DownloadStockData("MSFT",TimeSpan.FromDays(10)));
            MarketData.AddRange(DownloadStockData("AAPL", TimeSpan.FromDays(10)));
            MarketData.AddRange(DownloadStockData("YHOO", TimeSpan.FromDays(10)));

            // Convert stock data to dataset using encog-extensions
            DataSet dataSet = new DataSet().Convert(MarketData, "Market DataSet");

            // use encog-extensions to normalize the dataset 
            var analyst = new EncogAnalyst();
            var wizard = new AnalystWizard(analyst);
            wizard.Wizard(dataSet);

            // DataSet Goes In... 2D Double Array Comes Out... 
            var normalizer = new AnalystNormalizeDataSet(analyst);
            var normalizedData = normalizer.Normalize(dataSet);
         
            // Assert data is not null and differs from original
            Assert.IsNotNull(normalizedData);
            Assert.AreNotEqual(normalizedData[0, 0], dataSet.Tables[0].Rows[0][0]);

        }

        private static List<LoadedMarketData> DownloadStockData(string stockTickerSymbol,TimeSpan timeSpan)
        {
            IList<MarketDataType> dataNeeded = new List<MarketDataType>();
            dataNeeded.Add(MarketDataType.AdjustedClose);
            dataNeeded.Add(MarketDataType.Close);
            dataNeeded.Add(MarketDataType.Open);
            dataNeeded.Add(MarketDataType.High);
            dataNeeded.Add(MarketDataType.Low);
            dataNeeded.Add(MarketDataType.Volume);

            List<LoadedMarketData> MarketData =
                new YahooFinanceLoader().Load(
                    new TickerSymbol(stockTickerSymbol),
                    dataNeeded,
                    DateTime.Now.Subtract(timeSpan),
                    DateTime.Now).ToList();

            return MarketData;
        }
    }
}
