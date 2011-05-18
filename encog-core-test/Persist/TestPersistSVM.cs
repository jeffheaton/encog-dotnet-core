using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Encog.ML.SVM.Training;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util;
using System.IO;
using Encog.ML.SVM;
using Encog.ML.Data.Basic;
using Encog.ML.Data;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistSVM
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        private SupportVectorMachine Create()
        {
            MLDataSet training = new BasicMLDataSet(XOR.XORInput, XOR.XORIdeal);
            SupportVectorMachine result = new SupportVectorMachine(2, SVMType.EpsilonSupportVectorRegression, KernelType.RadialBasisFunction);
            SVMTrain train = new SVMTrain(result, training);
            train.Iteration();
            return result;
        }

        [TestMethod]
        public void TestPersistEG()
        {
            SupportVectorMachine network = Create();

            EncogDirectoryPersistence.SaveObject((EG_FILENAME), network);
            SupportVectorMachine network2 = (SupportVectorMachine)EncogDirectoryPersistence.LoadObject((EG_FILENAME));
            Validate(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            SupportVectorMachine network = Create();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            SupportVectorMachine network2 = (SupportVectorMachine)SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(network2);
        }

        private void Validate(SupportVectorMachine svm)
        {
            Assert.AreEqual(KernelType.RadialBasisFunction, svm.KernelType);
            Assert.AreEqual(SVMType.EpsilonSupportVectorRegression, svm.SVMType);
            Assert.AreEqual(4, svm.Model.SV.Length);
        }
    }
}
