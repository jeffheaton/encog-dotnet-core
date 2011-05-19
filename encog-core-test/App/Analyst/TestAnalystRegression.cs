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
