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
using Encog.Neural.Thermal;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistHopfield
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        [TestMethod]
        public void TestPersistEG()
        {
            HopfieldNetwork network = new HopfieldNetwork(4);
            network.SetWeight(1, 1, 1);
            network.SetProperty("x", 10);

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, network);
            HopfieldNetwork network2 = (HopfieldNetwork)EncogDirectoryPersistence.LoadObject((EG_FILENAME));

            ValidateHopfield(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            HopfieldNetwork network = new HopfieldNetwork(4);
            network.SetWeight(1, 1, 1);

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            HopfieldNetwork network2 = (HopfieldNetwork)SerializeObject.Load(SERIAL_FILENAME.ToString());

            ValidateHopfield(network2);
        }

        private void ValidateHopfield(HopfieldNetwork network)
        {
            Assert.AreEqual(4, network.NeuronCount);
            Assert.AreEqual(4, network.CurrentState.Count);
            Assert.AreEqual(16, network.Weights.Length);
            Assert.AreEqual(1.0, network.GetWeight(1, 1));
        }
    }
}
