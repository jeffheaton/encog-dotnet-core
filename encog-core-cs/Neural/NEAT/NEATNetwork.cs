//
// Encog(tm) Core v3.0 - .Net Version
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
using System.Collections.Generic;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util.Simple;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// Implements a NEAT network as a synapse between two layers. In Encog, a NEAT
    /// network is created by using a NEATSynapse between an input and output layer.
    /// NEAT networks only have an input and an output layer. There are no actual
    /// hidden layers. Rather this synapse will evolve many hidden neurons that have
    /// connections that are not easily defined by layers. Connections can be
    /// feedforward, recurrent, or self-connected.
    /// NEAT networks relieve the programmer of the need to define the hidden layer
    /// structure of the neural network.
    /// The output from the neural network can be calculated normally or using a
    /// snapshot. The snapshot mode is slower, but it can be more accurate. The
    /// snapshot handles recurrent layers better, as it takes the time to loop
    /// through the network multiple times to "flush out" the recurrent links.
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    [Serializable]
    public class NEATNetwork : BasicML, IMLContext, IMLRegression,
                               IMLError
    {
        /// <summary>
        /// The depth property.
        /// </summary>
        public const String PropertyNetworkDepth = "depth";

        /// <summary>
        /// The links property.
        /// </summary>
        public const String PropertyLinks = "links";

        /// <summary>
        /// The snapshot property.
        /// </summary>
        public const String PropertySnapshot = "snapshot";

        /// <summary>
        /// The neurons that make up this network.
        /// </summary>
        ///
        private readonly IList<NEATNeuron> _neurons;

        /// <summary>
        /// The activation function.
        /// </summary>
        ///
        private IActivationFunction _activationFunction;

        /// <summary>
        /// The input count.
        /// </summary>
        private int _inputCount;

        /// <summary>
        /// The depth of the network.
        /// </summary>
        ///
        private int _networkDepth;

        /// <summary>
        /// The output activation function.
        /// </summary>
        private IActivationFunction _outputActivationFunction;

        /// <summary>
        /// The output count.
        /// </summary>
        private int _outputCount;

        /// <summary>
        /// Should snapshot be used to calculate the output of the neural network.
        /// </summary>
        ///
        private bool _snapshot;

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public NEATNetwork()
        {
            _neurons = new List<NEATNeuron>();
            _snapshot = false;
        }

        /// <summary>
        /// Construct a NEAT synapse.
        /// </summary>
        ///
        /// <param name="inputCount">The number of input neurons.</param>
        /// <param name="outputCount">The number of output neurons.</param>
        /// <param name="neurons">The neurons in this synapse.</param>
        /// <param name="activationFunction">The activation function to use.</param>
        /// <param name="outputActivationFunction">The output activation function.</param>
        /// <param name="networkDepth">The depth of the network.</param>
        public NEATNetwork(int inputCount, int outputCount,
                           IEnumerable<NEATNeuron> neurons,
                           IActivationFunction activationFunction,
                           IActivationFunction outputActivationFunction,
                           int networkDepth)
        {
            _neurons = new List<NEATNeuron>();
            _snapshot = false;
            _inputCount = inputCount;
            _outputCount = outputCount;
            _outputActivationFunction = outputActivationFunction;

            foreach (NEATNeuron neuron in neurons)
            {
                _neurons.Add(neuron);
            }

            _networkDepth = networkDepth;
            _activationFunction = activationFunction;
        }

        /// <summary>
        /// Construct a NEAT network.
        /// </summary>
        ///
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputCount">The output count.</param>
        public NEATNetwork(int inputCount, int outputCount)
        {
            _neurons = new List<NEATNeuron>();
            _snapshot = false;
            _inputCount = inputCount;
            _outputCount = outputCount;
            _networkDepth = 0;
            _activationFunction = new ActivationSigmoid();
        }

        /// <summary>
        /// Set the activation function.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            get { return _activationFunction; }
            set { _activationFunction = value; }
        }

        /// <summary>
        /// The network depth.
        /// </summary>
        public int NetworkDepth
        {
            get { return _networkDepth; }
            set { _networkDepth = value; }
        }


        /// <value>The NEAT neurons.</value>
        public IList<NEATNeuron> Neurons
        {
            get { return _neurons; }
        }


        /// <summary>
        /// Sets if snapshot is used.
        /// </summary>
        public bool Snapshot
        {
            get { return _snapshot; }
            set { _snapshot = value; }
        }

        /// <value>the outputActivationFunction to set</value>
        public IActivationFunction OutputActivationFunction
        {
            get { return _outputActivationFunction; }
            set { _outputActivationFunction = value; }
        }

        #region MLContext Members

        /// <summary>
        /// Clear any context from previous runs. This sets the activation of all
        /// neurons to zero.
        /// </summary>
        ///
        public virtual void ClearContext()
        {
            foreach (NEATNeuron neuron  in  _neurons)
            {
                neuron.Output = 0;
            }
        }

        #endregion

        #region MLError Members

        /// <summary>
        /// Calculate the error for this neural network. 
        /// </summary>
        ///
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public virtual double CalculateError(IMLDataSet data)
        {
            return EncogUtility.CalculateRegressionError(this, data);
        }

        #endregion

        #region MLRegression Members

        /// <summary>
        /// Compute the output from this synapse.
        /// </summary>
        ///
        /// <param name="input">The input to this synapse.</param>
        /// <returns>The output from this synapse.</returns>
        public virtual IMLData Compute(IMLData input)
        {
            IMLData result = new BasicMLData(_outputCount);

            if (_neurons.Count == 0)
            {
                throw new NeuralNetworkError(
                    "This network has not been evolved yet, it has no neurons in the NEAT synapse.");
            }

            int flushCount = 1;

            if (_snapshot)
            {
                flushCount = _networkDepth;
            }

            // iterate through the network FlushCount times
            for (int i = 0; i < flushCount; ++i)
            {
                int outputIndex = 0;
                int index = 0;

                result.Clear();

                // populate the input neurons
                while (_neurons[index].NeuronType == NEATNeuronType.Input)
                {
                    _neurons[index].Output = input[index];

                    index++;
                }

                // set the bias neuron
                _neurons[index++].Output = 1;

                while (index < _neurons.Count)
                {
                    NEATNeuron currentNeuron = _neurons[index];

                    double sum = 0;


                    foreach (NEATLink link  in  currentNeuron.InboundLinks)
                    {
                        double weight = link.Weight;
                        double neuronOutput = link.FromNeuron.Output;
                        sum += weight*neuronOutput;
                    }

                    var d = new double[1];
                    d[0] = sum/currentNeuron.ActivationResponse;
                    _activationFunction.ActivationFunction(d, 0, d.Length);

                    _neurons[index].Output = d[0];

                    if (currentNeuron.NeuronType == NEATNeuronType.Output)
                    {
                        result[outputIndex++] = currentNeuron.Output;
                    }
                    index++;
                }
            }

            _outputActivationFunction.ActivationFunction(result.Data, 0,
                                                        result.Count);

            return result;
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public virtual int InputCount
        {
            get { return _inputCount; }
            set { _inputCount = value; }
        }

        /// <summary>
        /// The output count.
        /// </summary>
        public virtual int OutputCount
        {
            get { return _outputCount; }
            set { _outputCount = value; }
        }

        #endregion

        /// <summary>
        /// Not needed.
        /// </summary>
        public override void UpdateProperties()
        {
        }
    }
}
