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
using Encog.Neural.CPN;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistCPN
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        private CPNNetwork Create()
        {
            CPNNetwork result = new CPNNetwork(5, 4, 3, 2);
            return result;
        }

        [TestMethod]
        public void TestPersistEG()
        {
            CPNNetwork network = Create();

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, network);
            CPNNetwork network2 = (CPNNetwork)EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            Validate(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            CPNNetwork network = Create();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            CPNNetwork network2 = (CPNNetwork)SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(network2);
        }

        private void Validate(CPNNetwork cpn)
        {
            Assert.AreEqual(5, cpn.InputCount);
            Assert.AreEqual(4, cpn.InstarCount);
            Assert.AreEqual(3, cpn.OutputCount);
            Assert.AreEqual(3, cpn.OutstarCount);
            Assert.AreEqual(2, cpn.WinnerCount);
            Assert.AreEqual(5, cpn.WeightsInputToInstar.Rows);
            Assert.AreEqual(4, cpn.WeightsInputToInstar.Cols);
            Assert.AreEqual(4, cpn.WeightsInstarToOutstar.Rows);
            Assert.AreEqual(3, cpn.WeightsInstarToOutstar.Cols);
        }
    }
}
