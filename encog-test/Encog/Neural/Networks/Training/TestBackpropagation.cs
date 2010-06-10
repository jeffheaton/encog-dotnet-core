using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using encog_test.Neural.Networks;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Prune;

namespace encog_test.Neural.Networks.Training
{
    [TestFixture]
    public class TestBackpropagation
    {

        public void testBackpropagation()
        {
            INeuralDataSet trainingData = new BasicNeuralDataSet(XOR.XOR_INPUT, XOR.XOR_IDEAL);

            BasicNetwork network = CreateNetwork.createXORNetworkUntrained();
            ITrain train = new Backpropagation(network, trainingData, 0.7, 0.9);

            train.Iteration();
            double error1 = train.Error;
            train.Iteration();
            network = (BasicNetwork)train.Network;
            double error2 = train.Error;

            double improve = (error1 - error2) / error1;

            Assert.IsTrue(improve > 0.001);

        }

        [Test]
        public void testToString()
        {
            BasicNetwork network = CreateNetwork.createXORNetworkUntrained();
            ILayer input = network.GetLayer(BasicNetwork.TAG_INPUT);
            input.ToString();
        }

        [Test]
        public void testCounts()
        {
            BasicNetwork network = CreateNetwork.createXORNetworkUntrained();
            ILayer input = network.GetLayer(BasicNetwork.TAG_INPUT);
            input.ToString();
            Assert.AreEqual(6, network.CalculateNeuronCount());
        }

        [Test]
        public void testPrune()
        {
            BasicNetwork network = CreateNetwork.createXORNetworkUntrained();
            ILayer input = network.GetLayer(BasicNetwork.TAG_INPUT);
            ILayer hidden = input.Next[0].ToLayer;

            Assert.AreEqual(3, hidden.NeuronCount);

            PruneSelective prune = new PruneSelective(network);
            prune.Prune(hidden, 1);

            Assert.AreEqual(2, hidden.NeuronCount);
        }
    }
}
