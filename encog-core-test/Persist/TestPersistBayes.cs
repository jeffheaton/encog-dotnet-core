//
// Encog(tm) Core v3.2 - .Net Version (Unit Test)
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
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
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util;
using System.IO;
using Encog.ML.Bayesian;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistBayes
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        public BayesianNetwork Create()
        {
            BayesianNetwork network = new BayesianNetwork();
            BayesianEvent a = network.CreateEvent("a");
            BayesianEvent b = network.CreateEvent("b");

            network.CreateDependency(a, b);
            network.FinalizeStructure();
            a.Table.AddLine(0.5, true); // P(A) = 0.5
            b.Table.AddLine(0.2, true, true); // p(b|a) = 0.2
            b.Table.AddLine(0.8, true, false);// p(b|~a) = 0.8		
            network.Validate();
            return network;
        }

        public void Validate(BayesianNetwork network)
        {
            Assert.AreEqual(3, network.CalculateParameterCount());
        }

        [TestMethod]
        public void TestPersistEG()
        {
            BayesianNetwork network = Create();

            EncogDirectoryPersistence.SaveObject(EG_FILENAME, network);
            BayesianNetwork network2 = (BayesianNetwork)EncogDirectoryPersistence.LoadObject(EG_FILENAME);

            Validate(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            BayesianNetwork network = Create();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            BayesianNetwork network2 = (BayesianNetwork)SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(network2);
        }
    }
}
