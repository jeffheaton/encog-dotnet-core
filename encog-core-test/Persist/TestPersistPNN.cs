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
