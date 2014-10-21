//
// Encog(tm) Core v3.3 - .Net Version (unit test)
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
using System.IO;
using Encog.Neural.Freeform;
using Encog.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistFreeform
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        public FreeformNetwork Create()
        {
            FreeformNetwork network = XOR.CreateTrainedFreeformXOR();
            XOR.VerifyXOR(network, 0.1);

            network.SetProperty("test", "test2");

            return network;
        }

        public void Validate(FreeformNetwork network)
        {
            network.ClearContext();
            XOR.VerifyXOR(network, 0.1);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            var network = Create();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            var network2 = (FreeformNetwork)SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(network2);
        }
    }
}
