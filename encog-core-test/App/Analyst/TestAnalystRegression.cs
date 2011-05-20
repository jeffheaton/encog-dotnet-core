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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Encog.Util;
using Encog.Util.File;
using Encog.Util.CSV;

namespace Encog.App.Analyst
{
    [TestClass]
    public class TestAnalystRegression
    {
        public TempDir TEMP_DIR = new TempDir();

        [TestMethod]
        public void TestRegression()
        {
            FileInfo rawFile = TEMP_DIR.CreateFile("simple.csv");
            FileInfo egaFile = TEMP_DIR.CreateFile("simple.ega");
            FileInfo outputFile = TEMP_DIR.CreateFile("simple_output.csv");

            FileUtil.CopyResource("Encog.Resources.simple.csv", rawFile);
            FileUtil.CopyResource("Encog.Resources.simple-r.ega", egaFile);

            EncogAnalyst analyst = new EncogAnalyst();
            analyst.Load(egaFile);

            analyst.ExecuteTask("task-full");

            ReadCSV csv = new ReadCSV(outputFile.ToString(), true, CSVFormat.ENGLISH);
            while (csv.Next())
            {
                double diff = Math.Abs(csv.GetDouble(2) - csv.GetDouble(4));
                Assert.IsTrue(diff < 1.5);
            }

            Assert.AreEqual(4, analyst.Script.Fields.Length);
            Assert.AreEqual(3, analyst.Script.Fields[3].ClassMembers.Count);

            csv.Close();
        }
    }
}
