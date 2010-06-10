using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training;

namespace encog_test.Neural.Networks.Training
{
    [TestFixture]
    public class TestAnneal
    {
        [Test]
        public void testAnneal()
        {
            INeuralDataSet trainingData = new BasicNeuralDataSet(XOR.XOR_INPUT, XOR.XOR_IDEAL);
            BasicNetwork network = XOR.CreateThreeLayerNet();

            ICalculateScore score = new TrainingSetScore(trainingData);
            ITrain train = new NeuralSimulatedAnnealing(
                    network, score, 10, 2, 100);

            train.Iteration();
            double error1 = train.Error;
            train.Iteration();
            network = (BasicNetwork)train.Network;
            double error2 = train.Error;

            double improve = (error1 - error2) / error1;

            Assert.IsTrue(improve > 0.01);

        }
    }
}
