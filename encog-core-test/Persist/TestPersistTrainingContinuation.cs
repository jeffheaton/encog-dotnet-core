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
            MLDataSet trainingSet = XOR.CreateXORDataSet();
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
            MLDataSet trainingSet = XOR.CreateXORDataSet();
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
