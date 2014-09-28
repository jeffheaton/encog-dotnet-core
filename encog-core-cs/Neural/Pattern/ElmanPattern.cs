//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// This class is used to generate an Elman style recurrent neural network. This
    /// network type consists of three regular layers, an input output and hidden
    /// layer. There is also a context layer which accepts output from the hidden
    /// layer and outputs back to the hidden layer. This makes it a recurrent neural
    /// network.
    /// The Elman neural network is useful for temporal input data. The specified
    /// activation function will be used on all layers. The Elman neural network is
    /// similar to the Jordan neural network.
    /// </summary>
    ///
    public class ElmanPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The activation function.
        /// </summary>
        ///
        private IActivationFunction _activation;

        /// <summary>
        /// The number of hidden neurons.
        /// </summary>
        ///
        private int _hiddenNeurons;

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        ///
        private int _inputNeurons;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        ///
        private int _outputNeurons;

        /// <summary>
        /// Create an object to generate Elman neural networks.
        /// </summary>
        ///
        public ElmanPattern()
        {
            _inputNeurons = -1;
            _outputNeurons = -1;
            _hiddenNeurons = -1;
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer with the specified number of neurons.
        /// </summary>
        ///
        /// <param name="count">The number of neurons in this hidden layer.</param>
        public void AddHiddenLayer(int count)
        {
            if (_hiddenNeurons != -1)
            {
                throw new PatternError(
                    "An Elman neural network should have only one hidden layer.");
            }

            _hiddenNeurons = count;
        }

        /// <summary>
        /// Clear out any hidden neurons.
        /// </summary>
        ///
        public void Clear()
        {
            _hiddenNeurons = -1;
        }

        /// <summary>
        /// Generate the Elman neural network.
        /// </summary>
        ///
        /// <returns>The Elman neural network.</returns>
        public IMLMethod Generate()
        {
            BasicLayer hidden, input;

            var network = new BasicNetwork();
            network.AddLayer(input = new BasicLayer(_activation, true,
                                                    _inputNeurons));
            network.AddLayer(hidden = new BasicLayer(_activation, true,
                                                     _hiddenNeurons));
            network.AddLayer(new BasicLayer(null, false, _outputNeurons));
            input.ContextFedBy = hidden;
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }

        /// <summary>
        /// Set the activation function to use on each of the layers.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set { _activation = value; }
        }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        public int InputNeurons { set { _inputNeurons = value; } }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        public int OutputNeurons
        {

            set { _outputNeurons = value; }
        }

        #endregion
    }
}
