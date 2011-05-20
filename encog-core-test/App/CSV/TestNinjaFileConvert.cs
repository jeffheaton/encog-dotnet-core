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
using Encog.App.Quant.Ninja;
using Encog.Util;
using Encog.Util.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestNinjaFileConvert
    {
        public static readonly TempDir TempDir = new TempDir();
        public readonly FileInfo InputName = TempDir.CreateFile("test.csv");
        public readonly FileInfo OutputName = TempDir.CreateFile("test2.csv");


        public void GenerateTestFileHeadings(bool header)
        {
            var tw = new StreamWriter(InputName.ToString());
                
            if (header)
            {
                tw.WriteLine("date,time,open,high,low,close,volume");
            }
            tw.WriteLine("20100101,000000,10,12,8,9,1000");
            tw.WriteLine("20100102,000000,9,17,7,15,1000");


            // close the stream
            tw.Close();
        }

        [TestMethod]
        public void TestConvert()
        {
            GenerateTestFileHeadings(true);
            var norm = new NinjaFileConvert();
            norm.Analyze(InputName, true, CSVFormat.ENGLISH);
            norm.Process(OutputName.ToString());

            var tr = new StreamReader(OutputName.ToString());

            Assert.AreEqual("20100101 000000;10;12;8;9;1000", tr.ReadLine());
            Assert.AreEqual("20100102 000000;9;17;7;15;1000", tr.ReadLine());

            tr.Close();

            InputName.Delete();
            OutputName.Delete();
        }
    }
}
