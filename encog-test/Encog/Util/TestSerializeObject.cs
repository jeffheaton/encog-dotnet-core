using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Basic;
using NUnit.Framework;
using encog_test.Neural.Networks;
using Encog.Util;
using Encog.Neural.Networks.Pattern;
using Encog.Neural.Networks;
using System.IO;

namespace encog_test.Util
{
    [TestFixture]
    class TestSerializeObject
    {
        [Test]
        public void testSerializeXOR()
        {
            BasicNeuralDataSet set = new BasicNeuralDataSet(XOR.XOR_INPUT,
                    XOR.XOR_IDEAL);
            SerializeObject.Save("encog1.ser", set);
            set = (BasicNeuralDataSet)SerializeObject.Load("encog1.ser");
            XOR.TestXORDataSet(set);
            File.Delete("encog1.ser");
        }

        [Test]
        public void testSerializeNetwork()
        {
            RadialBasisPattern pattern = new RadialBasisPattern();
            pattern.InputNeurons = 1;
            pattern.AddHiddenLayer(2);
            pattern.OutputNeurons = 3;
            BasicNetwork net = pattern.Generate();

            SerializeObject.Save("encog2.ser", net);
            net = (BasicNetwork)SerializeObject.Load("encog2.ser");
            Assert.AreEqual(3, net.Structure.Layers.Count);
            File.Delete("encog2.ser");
        }


        [Test]
        public void testSerializeNetwork2()
        {
            ElmanPattern pattern = new ElmanPattern();
            pattern.InputNeurons = 1;
            pattern.AddHiddenLayer(2);
            pattern.OutputNeurons = 3;
            BasicNetwork net = pattern.Generate();

            SerializeObject.Save("encog3.ser", net);
            net = (BasicNetwork)SerializeObject.Load("encog3.ser");
            Assert.AreEqual(4, net.Structure.Layers.Count);
            File.Delete("encog3.ser");

        }
    }
}
