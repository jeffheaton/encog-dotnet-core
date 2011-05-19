using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util;
using System.IO;
using Encog.Util.File;
using Encog.App.Analyst.Wizard;
using Encog.App.Analyst.Report;

namespace Encog.App.Analyst
{
    [TestClass]
    public class TestAnalystWizard
    {
        public TempDir TEMP_DIR = new TempDir();

        [TestMethod]
        public void TestWizard()
        {
            FileInfo rawFile = TEMP_DIR.CreateFile("iris_raw.csv");
            FileUtil.CopyResource("Encog.Resources.iris.csv", rawFile);

            FileInfo analystFile = TEMP_DIR.CreateFile("iris.ega");

            EncogAnalyst encog = new EncogAnalyst();
            AnalystWizard wiz = new AnalystWizard(encog);
            wiz.Goal = AnalystGoal.Classification;
            wiz.Wizard(rawFile, true, AnalystFileFormat.DECPNT_COMMA);


            encog.ExecuteTask("task-full");

            encog.Save(analystFile);
            encog.Load(analystFile);

            AnalystReport report = new AnalystReport(encog);
            report.ProduceReport(TEMP_DIR.CreateFile("report.html"));

            Assert.AreEqual(5, encog.Script.Normalize.NormalizedFields.Count);
            Assert.AreEqual(4.3, encog.Script.Fields[0].Min, 0.001);
            Assert.AreEqual(7.9, encog.Script.Fields[0].Max, 0.001);
            Assert.AreEqual(5.84333, encog.Script.Fields[0].Mean, 0.001);
            Assert.AreEqual(encog.Script.Fields[0].Class, false);
            Assert.AreEqual(encog.Script.Fields[0].Real, true);
            Assert.AreEqual(encog.Script.Fields[0].Integer, false);
            Assert.AreEqual(encog.Script.Fields[0].Complete, true);
            Assert.AreEqual(-3.38833, encog.Script.Normalize.NormalizedFields[0].Normalize(0.001), 0.001);
        }
    }
}
