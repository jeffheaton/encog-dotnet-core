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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Encog.Util;
using System.IO;
using Encog.Neural.NEAT;
using Encog.Engine.Network.Activation;

namespace Encog.Persist
{
    [TestClass]
    public class TestPersistNEAT
    {
        public static readonly TempDir TEMP_DIR = new TempDir();
        public readonly FileInfo EG_FILENAME = TEMP_DIR.CreateFile("encogtest.eg");
        public readonly FileInfo SERIAL_FILENAME = TEMP_DIR.CreateFile("encogtest.ser");

        private NEATNetwork Create()
        {
            IList<NEATNeuron> neurons = new List<NEATNeuron>();
            IActivationFunction afSigmoid = new ActivationSigmoid();
            IActivationFunction afStep = new ActivationStep();

            // create the neurons
            NEATNeuron input1 = new NEATNeuron(
                    NEATNeuronType.Input,
                    1,
                    0.1,
                    0.2,
                    0.3);

            NEATNeuron input2 = new NEATNeuron(
                    NEATNeuronType.Input,
                    2,
                    0.1,
                    0.2,
                    0.3);

            NEATNeuron bias = new NEATNeuron(
                    NEATNeuronType.Bias,
                    3,
                    0.1,
                    0.2,
                    0.3);

            NEATNeuron hidden1 = new NEATNeuron(
                    NEATNeuronType.Hidden,
                    4,
                    0.1,
                    0.2,
                    0.3);

            NEATNeuron output = new NEATNeuron(
                    NEATNeuronType.Output,
                    5,
                    0.1,
                    0.2,
                    0.3);

            // add the neurons
            neurons.Add(input1);
            neurons.Add(input2);
            neurons.Add(hidden1);
            neurons.Add(bias);
            neurons.Add(output);

            // connect everything
            Link(0.01, input1, hidden1, false);
            Link(0.01, input2, hidden1, false);
            Link(0.01, bias, hidden1, false);
            Link(0.01, hidden1, output, false);

            // create the network
            NEATNetwork result = new NEATNetwork(2,
                    1,
                    neurons,
                    afSigmoid,
                    afStep,
                    3);

            return result;
        }

        private void Link(
                double weight, NEATNeuron from, NEATNeuron to, bool recurrent)
        {
            NEATLink l = new NEATLink(weight, from, to, recurrent);
            from.OutputboundLinks.Add(l);
            to.InboundLinks.Add(l);
        }

        [TestMethod]
        public void TestPersistEG()
        {
            NEATNetwork network = Create();

            EncogDirectoryPersistence.SaveObject((EG_FILENAME), network);
            NEATNetwork network2 = (NEATNetwork)EncogDirectoryPersistence.LoadObject((EG_FILENAME));

            Validate(network2);
        }

        [TestMethod]
        public void TestPersistSerial()
        {
            NEATNetwork network = Create();

            SerializeObject.Save(SERIAL_FILENAME.ToString(), network);
            NEATNetwork network2 = (NEATNetwork)SerializeObject.Load(SERIAL_FILENAME.ToString());

            Validate(network2);
        }

        private void Validate(NEATNetwork network)
        {
            Assert.AreEqual(1, network.OutputCount);
            Assert.AreEqual(2, network.InputCount);
            Assert.AreEqual(3, network.NetworkDepth);
            Assert.IsTrue(network.ActivationFunction is ActivationSigmoid);
            Assert.IsTrue(network.OutputActivationFunction is ActivationStep);
            Assert.AreEqual(0.0, ((ActivationStep)network.OutputActivationFunction).Center);
            Assert.AreEqual(1.0, ((ActivationStep)network.OutputActivationFunction).High);
            Assert.AreEqual(0.0, ((ActivationStep)network.OutputActivationFunction).Low);
            Assert.AreEqual(5, network.Neurons.Count);

            IDictionary<NEATNeuronType, NEATNeuron> neurons = new Dictionary<NEATNeuronType, NEATNeuron>();

            foreach (NEATNeuron neuron in network.Neurons)
            {
                neurons[neuron.NeuronType] = neuron;
            }

            Assert.AreEqual(4, neurons.Count);

            NEATNeuron output = neurons[NEATNeuronType.Output];
            NEATNeuron input = neurons[NEATNeuronType.Input];
            Assert.AreEqual(1, input.OutputboundLinks.Count);
            Assert.AreEqual(0, input.InboundLinks.Count);
            Assert.AreEqual(0, output.OutputboundLinks.Count);
            Assert.AreEqual(1, output.InboundLinks.Count);
        }
    }
}
