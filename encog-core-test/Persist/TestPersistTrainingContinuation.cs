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
using Encog.ML.Data;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistTrainingContinuation
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        [TestMethod]
        public void TestRPROPCont()
        {
            IMLDataSet trainingSet = XOR.CreateXORDataSet();
            BasicNetwork net1 = XOR.CreateUnTrainedXOR();
            BasicNetwork net2 = XOR.CreateUnTrainedXOR();

            ResilientPropagation rprop1 = new ResilientPropagation(net1, trainingSet);
            ResilientPropagation rprop2 = new ResilientPropagation(net2, trainingSet);

            rprop1.Iteration();
            rprop1.Iteration();

            rprop2.Iteration();
            rprop2.Iteration();

            TrainingContinuation cont = rprop2.Pause();

            ResilientPropagation rprop3 = new ResilientPropagation(net2, trainingSet);
            rprop3.Resume(cont);

            rprop1.Iteration();
            rprop3.Iteration();


            for (int i = 0; i < net1.Flat.Weights.Length; i++)
            {
                Assert.AreEqual(net1.Flat.Weights[i], net2.Flat.Weights[i], 0.0001);
            }
        }

        [TestMethod]
        public void TestRPROPContPersistEG()
        {
            IMLDataSet trainingSet = XOR.CreateXORDataSet();
            BasicNetwork net1 = XOR.CreateUnTrainedXOR();
            BasicNetwork net2 = XOR.CreateUnTrainedXOR();

            ResilientPropagation rprop1 = new ResilientPropagation(net1, trainingSet);
            ResilientPropagation rprop2 = new ResilientPropagation(net2, trainingSet);

            rprop1.Iteration();
            rprop1.Iteration();

            rprop2.Iteration();
            rprop2.Iteration();

            TrainingContinuation cont = rprop2.Pause();

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, cont);
            TrainingContinuation cont2 = (TrainingContinuation)EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            ResilientPropagation rprop3 = new ResilientPropagation(net2, trainingSet);
            rprop3.Resume(cont2);

            rprop1.Iteration();
            rprop3.Iteration();


            for (int i = 0; i < net1.Flat.Weights.Length; i++)
            {
                Assert.AreEqual(net1.Flat.Weights[i], net2.Flat.Weights[i], 0.0001);
            }
        }
    }
}
