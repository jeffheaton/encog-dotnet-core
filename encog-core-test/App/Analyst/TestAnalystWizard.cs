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
            wiz.Wizard(rawFile, true, AnalystFileFormat.DecpntComma);


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
