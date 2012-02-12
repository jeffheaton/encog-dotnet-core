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
using System.IO;
using Encog.App.Analyst.CSV.Sort;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestSort
    {
        public static readonly TempDir TempDir = new TempDir();
        public readonly FileInfo InputName = TempDir.CreateFile("test.csv");
        public readonly FileInfo OutputName = TempDir.CreateFile("test2.csv");

        public void GenerateTestFileHeadings(bool header)
        {
            var tw = new StreamWriter(InputName.ToString());

            if (header)
            {
                tw.WriteLine("a,b");
            }

            tw.WriteLine("five,5");
            tw.WriteLine("four,4");
            tw.WriteLine("two,2");
            tw.WriteLine("three,3");
            tw.WriteLine("six,6");
            tw.WriteLine("one,1");

            // close the stream
            tw.Close();
        }

        [TestMethod]
        public void TestSortHeaders()
        {
            GenerateTestFileHeadings(true);
            var norm = new SortCSV();
            norm.SortOrder.Add(new SortedField(1, SortType.SortString, true));
            norm.Process(InputName, OutputName, true, CSVFormat.English);

            var tr = new StreamReader(OutputName.ToString());

            Assert.AreEqual("\"a\",\"b\"", tr.ReadLine());
            Assert.AreEqual("\"one\",1", tr.ReadLine());
            Assert.AreEqual("\"two\",2", tr.ReadLine());
            Assert.AreEqual("\"three\",3", tr.ReadLine());
            Assert.AreEqual("\"four\",4", tr.ReadLine());
            Assert.AreEqual("\"five\",5", tr.ReadLine());
            Assert.AreEqual("\"six\",6", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());


            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }

        [TestMethod]
        public void TestSortNoHeaders()
        {
            GenerateTestFileHeadings(false);
            var norm = new SortCSV();
            norm.SortOrder.Add(new SortedField(1, SortType.SortInteger, true));
            norm.ProduceOutputHeaders = false;
            norm.Process(InputName, OutputName, false, CSVFormat.English);

            var tr = new StreamReader(OutputName.ToString());

            Assert.AreEqual("\"one\",1", tr.ReadLine());
            Assert.AreEqual("\"two\",2", tr.ReadLine());
            Assert.AreEqual("\"three\",3", tr.ReadLine());
            Assert.AreEqual("\"four\",4", tr.ReadLine());
            Assert.AreEqual("\"five\",5", tr.ReadLine());
            Assert.AreEqual("\"six\",6", tr.ReadLine());
            Assert.IsNull(tr.ReadLine());


            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }
    }
}
