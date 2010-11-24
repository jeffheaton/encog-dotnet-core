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
using Encog.Neural.NeuralData.Market.Loader;
using Encog.Neural.NeuralData.Market;
using Encog.Neural.NeuralData.Market.DB;

namespace encog_test.Data.Market
{
    [TestFixture]
    public class TestYahooLoader
    {
        [Test]
        public void Loader()
        {
            /*
            IMarketLoader loader = new YahooFinanceLoader();
            DateTime from = new DateTime(2008, 8, 4);
            DateTime to = new DateTime(2008, 8, 5);
            ICollection<StoredMarketData> list = loader.Load("aapl", null, from, to);

            IEnumerator<StoredMarketData> itr = list.GetEnumerator();
            itr.MoveNext();
            StoredMarketData data = itr.Current;
            Assert.AreEqual(160, (int)data.Close);
            itr.MoveNext();
            data = itr.Current;
            Assert.AreEqual(153, (int)data.Close);*/
        }
    }
}
