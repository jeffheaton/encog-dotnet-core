using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Pattern;
using Encog.Neural.Activation;
using Encog.Persist;
using System.IO;

namespace encog_test.Neural.Persist
{
    [TestFixture]
    public class TestPersist
    {
        private BasicNetwork getRBF()
        {
            RadialBasisPattern pattern = new RadialBasisPattern();
            pattern.InputNeurons = 1;
            pattern.AddHiddenLayer(2);
            pattern.OutputNeurons = 3;
            BasicNetwork net = pattern.Generate();
            return net;
        }

        private BasicNetwork getElman()
        {
            ElmanPattern pattern = new ElmanPattern();
            pattern.InputNeurons = 1;
            pattern.AddHiddenLayer(2);
            pattern.OutputNeurons = 3;
            pattern.ActivationFunction = new ActivationSigmoid();
            BasicNetwork net = pattern.Generate();
            return net;
        }

        [Test]
        public void testPersist()
        {
            File.Delete("encogtest.eg");
            EncogPersistedCollection encog =
                new EncogPersistedCollection("encogtest.eg", FileMode.OpenOrCreate);
            encog.Create();
            BasicNetwork net1 = getRBF();
            BasicNetwork net2 = getElman();
            encog.Add("rbf", net1);
            encog.Add("elman", net2);

            net1 = (BasicNetwork)encog.Find("rbf");
            net2 = (BasicNetwork)encog.Find("elman");

            Assert.IsNotNull(net1);
            Assert.IsNotNull(net2);
            File.Delete("encogtest.eg");

        }
    }
}
