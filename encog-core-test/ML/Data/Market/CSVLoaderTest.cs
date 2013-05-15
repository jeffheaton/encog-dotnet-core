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
using System.IO;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Encog.ML.Data.Market.Loader;
using Encog.ML.Data.Temporal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.ML.Data.Market
{
    [TestClass]
    public class CSVLoaderTest
    {



        static public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        [TestMethod]
        public void TestCSVLoader()
        {
            var loader = new CSVFinal();
            loader.DateFormat = "yyyy.MM.dd hh:mm:ss";

            var tickerAAPL = new TickerSymbol("AAPL", "NY");

            var desc = new MarketDataDescription(tickerAAPL, MarketDataType.Close, true, true);
            MarketMLDataSet marketData = new MarketMLDataSet(loader, 5, 1);
            marketData.AddDescription(desc);
            marketData.SequenceGrandularity = Util.Time.TimeUnit.Hours;
            var begin = new DateTime(2006, 1, 1);
            var end = new DateTime(2007, 7, 31);
            loader.GetFile((AssemblyDirectory + "\\smallCSV.csv"));
            marketData.Load(begin, end);
            marketData.Generate();
            // first test the points
            IEnumerator<TemporalPoint> itr = marketData.Points.GetEnumerator();
            itr.MoveNext();
            TemporalPoint point = itr.Current;

            Assert.AreEqual(0, point.Sequence);
            Assert.AreEqual(1, point.Data.Length);
            Assert.AreEqual(1.12884, point[0]);
            Assert.AreEqual(5, marketData.Points.Count);
        }
    }
}
