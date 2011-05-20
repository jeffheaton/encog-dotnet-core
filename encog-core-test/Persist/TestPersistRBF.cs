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
using Encog.Neural.RBF;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util;
using System.IO;
using Encog.ML.Data;
using Encog.MathUtil.RBF;
using Encog.Neural.Rbf.Training;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistRBF
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        [TestMethod]
        public void TestPersistEG()
        {
            MLDataSet trainingSet = XOR.CreateXORDataSet();
            RBFNetwork network = new RBFNetwork(2, 4, 1, RBFEnum.Gaussian);

            SVDTraining training = new SVDTraining(network, trainingSet);
            training.Iteration();
            XOR.VerifyXOR(network, 0.1);

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, network);
            RBFNetwork network2 = (RBFNetwork)EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            XOR.VerifyXOR(network2, 0.1);
        }
    }
}
