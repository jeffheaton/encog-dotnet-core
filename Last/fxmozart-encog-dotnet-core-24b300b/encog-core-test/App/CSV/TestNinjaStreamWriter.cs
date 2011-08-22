//
// Encog(tm) Unit Tests v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
//
using System;
using System.IO;
using Encog.App.Quant.Ninja;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestNinjaStreamWriter
    {
        public static readonly TempDir TempDir = new TempDir();
        public readonly FileInfo OutputName = TempDir.CreateFile("test2.csv");

        [TestMethod]
        public void TestWrite()
        {
            var nsw = new NinjaStreamWriter();

            nsw.Open(OutputName.ToString(), true, CSVFormat.English);

            nsw.BeginBar(new DateTime(2010, 01, 01));
            nsw.StoreColumn("close", 10);
            nsw.EndBar();

            nsw.BeginBar(new DateTime(2010, 01, 02));
            nsw.StoreColumn("close", 11);
            nsw.EndBar();

            nsw.Close();

            var tr = new StreamReader(OutputName.ToString());

            Assert.AreEqual("date,time,\"close\"", tr.ReadLine());
            Assert.AreEqual("20100101,0,10", tr.ReadLine());
            Assert.AreEqual("20100102,0,11", tr.ReadLine());

            tr.Close();
        }
    }
}
