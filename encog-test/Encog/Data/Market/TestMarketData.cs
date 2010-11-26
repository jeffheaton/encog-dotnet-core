// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.Neural.NeuralData.Temporal;
using Encog.Neural.NeuralData;

namespace encog_test.Data.Market
{
    [TestFixture]
    public class TestMarketData
    {
        [Test]
        public void MarketData()
        {
            /*IMarketLoader loader = new YahooFinanceLoader();
            String tickerAAPL = "AAPL";
            String tickerMSFT = "MSFT";
            MarketNeuralDataSet marketData = new MarketNeuralDataSet(loader, 5, 1);
            marketData.AddDescription(new MarketDataDescription(tickerAAPL, MarketDataType.CLOSE, true, true));
            marketData.AddDescription(new MarketDataDescription(tickerMSFT, MarketDataType.CLOSE, true, false));
            marketData.AddDescription(new MarketDataDescription(tickerAAPL, MarketDataType.VOLUME, true, false));
            marketData.AddDescription(new MarketDataDescription(tickerMSFT, MarketDataType.VOLUME, true, false));
            DateTime begin = new DateTime(2008, 7, 1);
            DateTime end = new DateTime(2008, 7, 31);
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
            Assert.AreEqual(39, (int)(point[2] / 1000000));
            Assert.AreEqual(100, (int)(point[3] / 1000000));

            itr.MoveNext();
            point = itr.Current;
            Assert.AreEqual(1, point.Sequence);
            Assert.AreEqual(4, point.Data.Length);
            Assert.AreEqual(168.18, point[0]);
            Assert.AreEqual(25.88, point[1]);
            Assert.AreEqual(29, (int)(point[2] / 1000000));
            Assert.AreEqual(84, (int)(point[3] / 1000000));

            itr.MoveNext();
            point = itr.Current;
            Assert.AreEqual(2, point.Sequence);
            Assert.AreEqual(4, point.Data.Length);
            Assert.AreEqual(170.12, point[0]);
            Assert.AreEqual(25.98, point[1]);
            Assert.AreEqual(18, (int)(point[2] / 1000000));
            Assert.AreEqual(37, (int)(point[3] / 1000000));

            // now check the actual data
            Assert.AreEqual(16, marketData.Data.Count);
            Assert.AreEqual(20, marketData.InputNeuronCount);
            Assert.AreEqual(1, marketData.OutputNeuronCount);

            IEnumerator<INeuralDataPair> itr2 = marketData.Data.GetEnumerator();
            itr2.MoveNext();
            INeuralDataPair pair = itr2.Current;
            Assert.AreEqual(20, pair.Input.Count);
            Assert.AreEqual(1, pair.Ideal.Count);

            Assert.AreEqual(-0.037, Math.Round(pair.Input[0] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.037, Math.Round(pair.Input[1] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.246, Math.Round(pair.Input[2] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.156, Math.Round(pair.Input[3] * 1000.0) / 1000.0);
            Assert.AreEqual(0.012, Math.Round(pair.Input[4] * 1000.0) / 1000.0);
            Assert.AreEqual(0.0040, Math.Round(pair.Input[5] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.375, Math.Round(pair.Input[6] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.562, Math.Round(pair.Input[7] * 1000.0) / 1000.0);
            Assert.AreEqual(0.03, Math.Round(pair.Input[8] * 1000.0) / 1000.0);
            Assert.AreEqual(0.0020, Math.Round(pair.Input[9] * 1000.0) / 1000.0);
            Assert.AreEqual(0.57, Math.Round(pair.Input[10] * 100.0) / 100.0);
            Assert.AreEqual(0.929, Math.Round(pair.Input[11] * 1000.0) / 1000.0);
            Assert.AreEqual(0.025, Math.Round(pair.Input[12] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.0070, Math.Round(pair.Input[13] * 1000.0) / 1000.0);
            // for some reason, Yahoo likes to vary the volume numbers slightly, sometimes!
            Assert.AreEqual(0.1, Math.Round(pair.Input[14] * 10.0) / 10.0);
            Assert.AreEqual(-0.084, Math.Round(pair.Input[15] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.03, Math.Round(pair.Input[16] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.024, Math.Round(pair.Input[17] * 1000.0) / 1000.0);
            Assert.AreEqual(0.008, Math.Round(pair.Input[18] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.172, Math.Round(pair.Input[19] * 1000.0) / 1000.0);

            itr2.MoveNext();
            pair = itr2.Current;
            Assert.AreEqual(20, pair.Input.Count);
            Assert.AreEqual(1, pair.Ideal.Count);

            Assert.AreEqual(0.012, Math.Round(pair.Input[0] * 1000.0) / 1000.0);
            Assert.AreEqual(0.0040, Math.Round(pair.Input[1] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.375, Math.Round(pair.Input[2] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.562, Math.Round(pair.Input[3] * 1000.0) / 1000.0);
            Assert.AreEqual(0.03, Math.Round(pair.Input[4] * 1000.0) / 1000.0);
            Assert.AreEqual(0.0020, Math.Round(pair.Input[5] * 1000.0) / 1000.0);
            Assert.AreEqual(0.6, Math.Round(pair.Input[6] * 10.0) / 10.0);
            Assert.AreEqual(0.929, Math.Round(pair.Input[7] * 1000.0) / 1000.0);
            Assert.AreEqual(0.025, Math.Round(pair.Input[8] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.0070, Math.Round(pair.Input[9] * 1000.0) / 1000.0);
            Assert.AreEqual(0.1, Math.Round(pair.Input[10] * 10.0) / 10.0);
            Assert.AreEqual(-0.084, Math.Round(pair.Input[11] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.03, Math.Round(pair.Input[12] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.024, Math.Round(pair.Input[13] * 1000.0) / 1000.0);
            Assert.AreEqual(0.0080, Math.Round(pair.Input[14] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.172, Math.Round(pair.Input[15] * 1000.0) / 1000.0);
            Assert.AreEqual(0.014, Math.Round(pair.Input[16] * 1000.0) / 1000.0);
            Assert.AreEqual(0.0090, Math.Round(pair.Input[17] * 1000.0) / 1000.0);
            Assert.AreEqual(-0.1, Math.Round(pair.Input[18] * 10.0) / 10.0);
            Assert.AreEqual(0.066, Math.Round(pair.Input[19] * 1000.0) / 1000.0);
		*/

        }
    }
}
