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
using Encog.MathUtil.Randomize;
using Encog.ML;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Construct an ADALINE neural network.
    /// </summary>
    ///
    public class ADALINEPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of neurons in the input layer.
        /// </summary>
        ///
        private int _inputNeurons;

        /// <summary>
        /// The number of neurons in the output layer.
        /// </summary>
        ///
        private int _outputNeurons;

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Not used, the ADALINE has no hidden layers, this will throw an error.
        /// </summary>
        ///
        /// <param name="count">The neuron count.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("An ADALINE network has no hidden layers.");
        }

        /// <summary>
        /// Clear out any parameters.
        /// </summary>
        ///
        public void Clear()
        {
            _inputNeurons = 0;
            _outputNeurons = 0;
        }

        /// <summary>
        /// Generate the network.
        /// </summary>
        public IMLMethod Generate()
        {
            var network = new BasicNetwork();

            ILayer inputLayer = new BasicLayer(new ActivationLinear(), true,
                                              _inputNeurons);
            ILayer outputLayer = new BasicLayer(new ActivationLinear(), false,
                                               _outputNeurons);

            network.AddLayer(inputLayer);
            network.AddLayer(outputLayer);
            network.Structure.FinalizeStructure();

            (new RangeRandomizer(-0.5d, 0.5d)).Randomize(network);

            return network;
        }

        /// <summary>
        /// Not used, ADALINE does not use custom activation functions.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A ADALINE network can't specify a custom activation function.");
            }
        }


        /// <summary>
        /// Set the input neurons.
        /// </summary>
        public int InputNeurons
        {
            set { _inputNeurons = value; }
        }


        /// <summary>
        /// Set the output neurons.
        /// </summary>
        public int OutputNeurons
        {
            set { _outputNeurons = value; }
        }

        #endregion
    }
}
