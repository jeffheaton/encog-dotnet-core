//
// Encog(tm) Core v3.2 - .Net Version (Unit Test)
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
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
