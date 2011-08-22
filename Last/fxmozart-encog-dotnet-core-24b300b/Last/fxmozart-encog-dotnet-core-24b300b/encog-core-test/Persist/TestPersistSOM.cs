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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util;
using System.IO;
using Encog.Neural.SOM;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistSOM
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        private SOMNetwork Create()
        {
            SOMNetwork network = new SOMNetwork(4, 2);
            return network;
        }

        [TestMethod]
        public void TestPersistEG()
        {
            SOMNetwork network = Create();

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, network);
            SOMNetwork network2 = (SOMNetwork)EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            Validate(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            SOMNetwork network = Create();
            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            SOMNetwork network2 = (SOMNetwork)SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(network2);
        }

        private void Validate(SOMNetwork network)
        {
            Assert.AreEqual(4, network.InputCount);
            Assert.AreEqual(2, network.OutputCount);
            Assert.AreEqual(8, network.Weights.ToPackedArray().Length);
        }
    }
}
