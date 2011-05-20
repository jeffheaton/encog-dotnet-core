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
