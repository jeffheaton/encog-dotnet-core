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
using Encog.Neural.BAM;
using Encog.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistBAM
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        private BAMNetwork Create()
        {
            var network = new BAMNetwork(6, 3);
            network.WeightsF1ToF2[1, 1] = 2.0;
            network.WeightsF2ToF1[2, 2] = 3.0;
            return network;
        }

        [TestMethod]
        public void TestPersistEG()
        {
            BAMNetwork network = Create();

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, network);
            var network2 = (BAMNetwork) EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            ValidateBAM(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            BAMNetwork network = Create();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            var network2 = (BAMNetwork) SerializeObject.Load(SERIAL_FILENAME.ToString());

            ValidateBAM(network2);
        }

        private void ValidateBAM(BAMNetwork network)
        {
            Assert.AreEqual(6, network.F1Count);
            Assert.AreEqual(3, network.F2Count);
            Assert.AreEqual(18, network.WeightsF1ToF2.Size);
            Assert.AreEqual(18, network.WeightsF2ToF1.Size);
            Assert.AreEqual(2.0, network.WeightsF1ToF2[1, 1]);
            Assert.AreEqual(3.0, network.WeightsF2ToF1[2, 2]);
        }
    }
}
