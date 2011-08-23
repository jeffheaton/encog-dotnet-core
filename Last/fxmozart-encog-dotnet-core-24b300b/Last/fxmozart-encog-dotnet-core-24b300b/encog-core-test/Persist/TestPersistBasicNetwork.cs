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
using Encog.Neural.Networks;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistBasicNetwork
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        public BasicNetwork Create()
        {
            BasicNetwork network = XOR.CreateTrainedXOR();
            XOR.VerifyXOR(network, 0.1);

            network.SetProperty("test", "test2");


            return network;
        }

        public void Validate(BasicNetwork network)
        {
            network.ClearContext();
            XOR.VerifyXOR(network, 0.1);
        }
        [TestMethod]
        public void TestPersistEG()
        {
            BasicNetwork network = Create();

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, network);
            BasicNetwork network2 = (BasicNetwork)EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            Validate(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            BasicNetwork network = Create();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            BasicNetwork network2 = (BasicNetwork)SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(network2);
        }


    }
}
