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
using Encog.Neural.Networks;
using Encog.Util.Simple;
using Encog.Neural.Networks.Structure;
using Encog.Neural.Flat;
using Encog.Util;
using Encog.ML.Data.Basic;

namespace Encog.Neural.Prune
{
    /// <summary>
    /// Summary description for TestPruneSelective
    /// </summary>
    [TestClass]
    public class TestPruneSelective
    {
        public TestPruneSelective()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        
        private BasicNetwork ObtainNetwork()
        {
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 3, 0, 4, false);
            double[] weights = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            NetworkCODEC.ArrayToNetwork(weights, network);

            Assert.AreEqual(1.0, network.GetWeight(1, 0, 0), 0.01);
            Assert.AreEqual(2.0, network.GetWeight(1, 1, 0), 0.01);
            Assert.AreEqual(3.0, network.GetWeight(1, 2, 0), 0.01);
            Assert.AreEqual(4.0, network.GetWeight(1, 3, 0), 0.01);

            Assert.AreEqual(5.0, network.GetWeight(1, 0, 1), 0.01);
            Assert.AreEqual(6.0, network.GetWeight(1, 1, 1), 0.01);
            Assert.AreEqual(7.0, network.GetWeight(1, 2, 1), 0.01);
            Assert.AreEqual(8.0, network.GetWeight(1, 3, 1), 0.01);

            Assert.AreEqual(9.0, network.GetWeight(1, 0, 2), 0.01);
            Assert.AreEqual(10.0, network.GetWeight(1, 1, 2), 0.01);
            Assert.AreEqual(11.0, network.GetWeight(1, 2, 2), 0.01);
            Assert.AreEqual(12.0, network.GetWeight(1, 3, 2), 0.01);

            Assert.AreEqual(13.0, network.GetWeight(1, 0, 3), 0.01);
            Assert.AreEqual(14.0, network.GetWeight(1, 1, 3), 0.01);
            Assert.AreEqual(15.0, network.GetWeight(1, 2, 3), 0.01);
            Assert.AreEqual(16.0, network.GetWeight(1, 3, 3), 0.01);

            Assert.AreEqual(17.0, network.GetWeight(0, 0, 0), 0.01);
            Assert.AreEqual(18.0, network.GetWeight(0, 1, 0), 0.01);
            Assert.AreEqual(19.0, network.GetWeight(0, 2, 0), 0.01);
            Assert.AreEqual(20.0, network.GetWeight(0, 0, 1), 0.01);
            Assert.AreEqual(21.0, network.GetWeight(0, 1, 1), 0.01);
            Assert.AreEqual(22.0, network.GetWeight(0, 2, 1), 0.01);

            Assert.AreEqual(20.0, network.GetWeight(0, 0, 1), 0.01);
            Assert.AreEqual(21.0, network.GetWeight(0, 1, 1), 0.01);
            Assert.AreEqual(22.0, network.GetWeight(0, 2, 1), 0.01);

            Assert.AreEqual(23.0, network.GetWeight(0, 0, 2), 0.01);
            Assert.AreEqual(24.0, network.GetWeight(0, 1, 2), 0.01);
            Assert.AreEqual(25.0, network.GetWeight(0, 2, 2), 0.01);


            return network;
        }

        private void CheckWithModel(FlatNetwork model, FlatNetwork pruned)
        {
            Assert.AreEqual(model.Weights.Length, pruned.Weights.Length);
            Assert.AreEqual(model.ContextTargetOffset, pruned.ContextTargetOffset);
            Assert.AreEqual(model.ContextTargetSize, pruned.ContextTargetSize);
            Assert.AreEqual(model.LayerCounts, pruned.LayerCounts);
            Assert.AreEqual(model.LayerFeedCounts, pruned.LayerFeedCounts);
            Assert.AreEqual(model.LayerIndex, pruned.LayerIndex);
            Assert.AreEqual(model.LayerOutput.Length, pruned.LayerOutput.Length);
            Assert.AreEqual(model.WeightIndex, pruned.WeightIndex);
        }

        [TestMethod]
        public void TestPruneNeuronInput()
        {
            BasicNetwork network = ObtainNetwork();
            Assert.AreEqual(2, network.InputCount);
            PruneSelective prune = new PruneSelective(network);
            prune.Prune(0, 1);
            Assert.AreEqual(22, network.EncodedArrayLength());
            Assert.AreEqual(1, network.GetLayerNeuronCount(0));
            Assert.AreEqual("1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,19,20,22,23,25", network.DumpWeights());

            BasicNetwork model = EncogUtility.SimpleFeedForward(1, 3, 0, 4, false);
            CheckWithModel(model.Structure.Flat, network.Structure.Flat);
            Assert.AreEqual(1, network.InputCount);
        }

        [TestMethod]
        public void TestPruneNeuronHidden()
        {
            BasicNetwork network = ObtainNetwork();
            PruneSelective prune = new PruneSelective(network);
            prune.Prune(1, 1);
            Assert.AreEqual(18, network.EncodedArrayLength());
            Assert.AreEqual(2, network.GetLayerNeuronCount(1));
            Assert.AreEqual("1,3,4,5,7,8,9,11,12,13,15,16,17,18,19,23,24,25", network.DumpWeights());

            BasicNetwork model = EncogUtility.SimpleFeedForward(2, 2, 0, 4, false);
            CheckWithModel(model.Structure.Flat, network.Structure.Flat);
        }

        [TestMethod]
        public void TestPruneNeuronOutput()
        {
            BasicNetwork network = ObtainNetwork();
            Assert.AreEqual(4, network.OutputCount);
            PruneSelective prune = new PruneSelective(network);
            prune.Prune(2, 1);
            Assert.AreEqual(21, network.EncodedArrayLength());
            Assert.AreEqual(3, network.GetLayerNeuronCount(2));
            Assert.AreEqual("1,2,3,4,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25", network.DumpWeights());

            BasicNetwork model = EncogUtility.SimpleFeedForward(2, 3, 0, 3, false);
            CheckWithModel(model.Structure.Flat, network.Structure.Flat);
            Assert.AreEqual(3, network.OutputCount);
        }

        [TestMethod]
        public void TestNeuronSignificance()
        {
            BasicNetwork network = ObtainNetwork();
            PruneSelective prune = new PruneSelective(network);
            double inputSig = prune.DetermineNeuronSignificance(0, 1);
            double hiddenSig = prune.DetermineNeuronSignificance(1, 1);
            double outputSig = prune.DetermineNeuronSignificance(2, 1);
            Assert.AreEqual(63.0, inputSig, 0.01);
            Assert.AreEqual(95.0, hiddenSig, 0.01);
            Assert.AreEqual(26.0, outputSig, 0.01);
        }

        [TestMethod]
        public void TestIncreaseNeuronCountHidden()
        {
            BasicNetwork network = XOR.CreateTrainedXOR();
            Assert.IsTrue(XOR.VerifyXOR(network, 0.10));
            PruneSelective prune = new PruneSelective(network);
            prune.ChangeNeuronCount(1, 5);

            BasicNetwork model = EncogUtility.SimpleFeedForward(2, 5, 0, 1, false);
            CheckWithModel(model.Structure.Flat, network.Structure.Flat);

            Assert.IsTrue(XOR.VerifyXOR(network, 0.10));
        }

        [TestMethod]
        public void TestIncreaseNeuronCountHidden2()
        {
            BasicNetwork network = EncogUtility.SimpleFeedForward(5, 6, 0, 2, true);
            PruneSelective prune = new PruneSelective(network);
            prune.ChangeNeuronCount(1, 60);

            BasicMLData input = new BasicMLData(5);
            BasicNetwork model = EncogUtility.SimpleFeedForward(5, 60, 0, 2, true);
            CheckWithModel(model.Structure.Flat, network.Structure.Flat);
            model.Compute(input);
            network.Compute(input);
        }

        [TestMethod]
        public void TestRandomizeNeuronInput()
        {
            double[] d = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 3, 0, 1, false);
            NetworkCODEC.ArrayToNetwork(d, network);
            PruneSelective prune = new PruneSelective(network);
            prune.RandomizeNeuron(100, 100, 0, 1);
            Assert.AreEqual("0,0,0,0,0,100,0,0,100,0,0,100,0", network.DumpWeights());
        }

        [TestMethod]
        public void TestRandomizeNeuronHidden()
        {
            double[] d = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 3, 0, 1, false);
            NetworkCODEC.ArrayToNetwork(d, network);
            PruneSelective prune = new PruneSelective(network);
            prune.RandomizeNeuron(100, 100, 1, 1);
            Assert.AreEqual("0,100,0,0,0,0,0,100,100,100,0,0,0", network.DumpWeights());
        }

        [TestMethod]
        public void TestRandomizeNeuronOutput()
        {
            double[] d = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 3, 0, 1, false);
            NetworkCODEC.ArrayToNetwork(d, network);
            PruneSelective prune = new PruneSelective(network);
            prune.RandomizeNeuron(100, 100, 2, 0);
            Assert.AreEqual("100,100,100,100,0,0,0,0,0,0,0,0,0", network.DumpWeights());
        }
    }
}
