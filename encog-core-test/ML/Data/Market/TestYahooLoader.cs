using System;
using System.Collections.Generic;
using Encog.ML.Data.Market.Loader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.Data.Market
{
    [TestClass]
    public class TestYahooLoader
    {
        [TestMethod]
        public void Loader()
        {
            IMarketLoader loader = new YahooFinanceLoader();
            var from = new DateTime(2008, 8, 4);
            var to = new DateTime(2008, 8, 5);
            ICollection<LoadedMarketData> list = loader.Load(new TickerSymbol("aapl"), null, from, to);

            IEnumerator<LoadedMarketData> itr = list.GetEnumerator();
            itr.MoveNext();
            LoadedMarketData data = itr.Current;
            Assert.AreEqual(160, (int) data.GetData(MarketDataType.Close));
            itr.MoveNext();
            data = itr.Current;
            Assert.AreEqual(153, (int) data.GetData(MarketDataType.Close));
        }
    }
}