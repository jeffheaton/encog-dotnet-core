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
using Encog.ML.Data.Temporal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.Data.Market
{
    [TestClass]
    public class TestMarketData
    {
        [TestMethod]
        public void MarketData()
        {
            IMarketLoader loader = new YahooFinanceLoader();
            var tickerAAPL = new TickerSymbol("AAPL", null);
            var tickerMSFT = new TickerSymbol("MSFT", null);
            var marketData = new MarketMLDataSet(loader, 5, 1);
            marketData.AddDescription(new MarketDataDescription(tickerAAPL, MarketDataType.Close, true, true));
            marketData.AddDescription(new MarketDataDescription(tickerMSFT, MarketDataType.Close, true, false));
            marketData.AddDescription(new MarketDataDescription(tickerAAPL, MarketDataType.Volume, true, false));
            marketData.AddDescription(new MarketDataDescription(tickerMSFT, MarketDataType.Volume, true, false));
            var begin = new DateTime(2008, 7, 1);
            var end = new DateTime(2008, 7, 31);
            marketData.Load(begin, end);
            marketData.Generate();
            Assert.AreEqual(22, marketData.Points.Count);

            // first test the points
            IEnumerator<TemporalPoint> itr = marketData.Points.GetEnumerator();
            itr.MoveNext();
            TemporalPoint point = itr.Current;

            Assert.AreEqual(0, point.Sequence);
            Assert.AreEqual(4, point.Data.Length);
            Assert.AreEqual(174.68, point[0]);
            Assert.AreEqual(26.87, point[1]);
            Assert.AreEqual(39, (int) (point[2]/1000000));
            Assert.AreEqual(100, (int) (point[3]/1000000));

            itr.MoveNext();
            point = itr.Current;
            Assert.AreEqual(1, point.Sequence);
            Assert.AreEqual(4, point.Data.Length);
            Assert.AreEqual(168.18, point[0]);
            Assert.AreEqual(25.88, point[1]);
            Assert.AreEqual(29, (int) (point[2]/1000000));
            Assert.AreEqual(84, (int) (point[3]/1000000));

            itr.MoveNext();
            point = itr.Current;
            Assert.AreEqual(2, point.Sequence);
            Assert.AreEqual(4, point.Data.Length);
            Assert.AreEqual(170.12, point[0]);
            Assert.AreEqual(25.98, point[1]);
            Assert.AreEqual(18, (int) (point[2]/1000000));
            Assert.AreEqual(37, (int) (point[3]/1000000));

            // now check the actual data
            Assert.AreEqual(16, marketData.Data.Count);
            Assert.AreEqual(20, marketData.InputNeuronCount);
            Assert.AreEqual(1, marketData.OutputNeuronCount);

            IEnumerator<IMLDataPair> itr2 = marketData.Data.GetEnumerator();
            itr2.MoveNext();
            IMLDataPair pair = itr2.Current;
            Assert.AreEqual(20, pair.Input.Count);
            Assert.AreEqual(1, pair.Ideal.Count);

            Assert.AreEqual(-0.037, Math.Round(pair.Input[0]*1000.0)/1000.0);
            Assert.AreEqual(-0.037, Math.Round(pair.Input[1]*1000.0)/1000.0);
            Assert.AreEqual(-0.246, Math.Round(pair.Input[2]*1000.0)/1000.0);
            Assert.AreEqual(-0.156, Math.Round(pair.Input[3]*1000.0)/1000.0);
            Assert.AreEqual(0.012, Math.Round(pair.Input[4]*1000.0)/1000.0);
            Assert.AreEqual(0.0040, Math.Round(pair.Input[5]*1000.0)/1000.0);
            Assert.AreEqual(-0.375, Math.Round(pair.Input[6]*1000.0)/1000.0);
            Assert.AreEqual(-0.562, Math.Round(pair.Input[7]*1000.0)/1000.0);
            Assert.AreEqual(0.03, Math.Round(pair.Input[8]*1000.0)/1000.0);
            Assert.AreEqual(0.0020, Math.Round(pair.Input[9]*1000.0)/1000.0);
            Assert.AreEqual(0.57, Math.Round(pair.Input[10]*100.0)/100.0);
            Assert.AreEqual(0.929, Math.Round(pair.Input[11]*1000.0)/1000.0);
            Assert.AreEqual(0.025, Math.Round(pair.Input[12]*1000.0)/1000.0);
            Assert.AreEqual(-0.0070, Math.Round(pair.Input[13]*1000.0)/1000.0);
            // for some reason, Yahoo likes to vary the volume numbers slightly, sometimes!
            Assert.AreEqual(0.1, Math.Round(pair.Input[14]*10.0)/10.0);
            Assert.AreEqual(-0.084, Math.Round(pair.Input[15]*1000.0)/1000.0);
            Assert.AreEqual(-0.03, Math.Round(pair.Input[16]*1000.0)/1000.0);
            Assert.AreEqual(-0.024, Math.Round(pair.Input[17]*1000.0)/1000.0);
            Assert.AreEqual(0.008, Math.Round(pair.Input[18]*1000.0)/1000.0);
            Assert.AreEqual(-0.172, Math.Round(pair.Input[19]*1000.0)/1000.0);

            itr2.MoveNext();
            pair = itr2.Current;
            Assert.AreEqual(20, pair.Input.Count);
            Assert.AreEqual(1, pair.Ideal.Count);

            Assert.AreEqual(0.012, Math.Round(pair.Input[0]*1000.0)/1000.0);
            Assert.AreEqual(0.0040, Math.Round(pair.Input[1]*1000.0)/1000.0);
            Assert.AreEqual(-0.375, Math.Round(pair.Input[2]*1000.0)/1000.0);
            Assert.AreEqual(-0.562, Math.Round(pair.Input[3]*1000.0)/1000.0);
            Assert.AreEqual(0.03, Math.Round(pair.Input[4]*1000.0)/1000.0);
            Assert.AreEqual(0.0020, Math.Round(pair.Input[5]*1000.0)/1000.0);
            Assert.AreEqual(0.6, Math.Round(pair.Input[6]*10.0)/10.0);
            Assert.AreEqual(0.929, Math.Round(pair.Input[7]*1000.0)/1000.0);
            Assert.AreEqual(0.025, Math.Round(pair.Input[8]*1000.0)/1000.0);
            Assert.AreEqual(-0.0070, Math.Round(pair.Input[9]*1000.0)/1000.0);
            Assert.AreEqual(0.1, Math.Round(pair.Input[10]*10.0)/10.0);
            Assert.AreEqual(-0.084, Math.Round(pair.Input[11]*1000.0)/1000.0);
            Assert.AreEqual(-0.03, Math.Round(pair.Input[12]*1000.0)/1000.0);
            Assert.AreEqual(-0.024, Math.Round(pair.Input[13]*1000.0)/1000.0);
            Assert.AreEqual(0.0080, Math.Round(pair.Input[14]*1000.0)/1000.0);
            Assert.AreEqual(-0.172, Math.Round(pair.Input[15]*1000.0)/1000.0);
            Assert.AreEqual(0.014, Math.Round(pair.Input[16]*1000.0)/1000.0);
            Assert.AreEqual(0.0090, Math.Round(pair.Input[17]*1000.0)/1000.0);
            Assert.AreEqual(-0.1, Math.Round(pair.Input[18]*10.0)/10.0);
            Assert.AreEqual(0.066, Math.Round(pair.Input[19]*1000.0)/1000.0);
        }
    }
}
