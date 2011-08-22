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
using Encog.Neural.PNN;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks.Training.PNN;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistPNN
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        public BasicPNN create()
        {
            PNNOutputMode mode = PNNOutputMode.Regression;

            BasicPNN network = new BasicPNN(PNNKernelType.Gaussian, mode, 2, 1);

            BasicMLDataSet trainingSet = new BasicMLDataSet(XOR.XORInput,
                    XOR.XORIdeal);

            TrainBasicPNN train = new TrainBasicPNN(network, trainingSet);
            train.Iteration();
            XOR.VerifyXOR(network, 0.001);
            return network;
        }

        [TestMethod]
        public void TestPersistEG()
        {
            BasicPNN network = create();

            EncogDirectoryPersistence.SaveObject((EG_FILENAME), network);
            BasicPNN network2 = (BasicPNN)EncogDirectoryPersistence.LoadObject((EG_FILENAME));

            XOR.VerifyXOR(network2, 0.001);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            BasicPNN network = create();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            BasicPNN network2 = (BasicPNN)SerializeObject.Load(SERIAL_FILENAME.ToString());

            XOR.VerifyXOR(network2, 0.001);
        }
    }
}
