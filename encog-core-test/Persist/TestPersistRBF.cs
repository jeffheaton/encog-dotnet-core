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
