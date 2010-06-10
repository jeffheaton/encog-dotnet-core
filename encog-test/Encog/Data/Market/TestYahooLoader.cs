using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.Neural.NeuralData.Market.Loader;
using Encog.Neural.NeuralData.Market;

namespace encog_test.Data.Market
{
    [TestFixture]
    public class TestYahooLoader
    {
        [Test]
        public void Loader()
        {
            IMarketLoader loader = new YahooFinanceLoader();
            DateTime from = new DateTime(2008, 8, 4);
            DateTime to = new DateTime(2008, 8, 5);
            ICollection<LoadedMarketData> list = loader.Load(new TickerSymbol("aapl"), null, from, to);

            IEnumerator<LoadedMarketData> itr = list.GetEnumerator();
            itr.MoveNext();
            LoadedMarketData data = itr.Current;
            Assert.AreEqual(160, (int)data.GetData(MarketDataType.CLOSE));
            itr.MoveNext();
            data = itr.Current;
            Assert.AreEqual(153, (int)data.GetData(MarketDataType.CLOSE));
        }
    }
}
