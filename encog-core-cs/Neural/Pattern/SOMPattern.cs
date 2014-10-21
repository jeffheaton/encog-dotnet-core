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
using Encog.Neural.SOM;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// A self organizing map is a neural network pattern with an input and output
    /// layer. There is no hidden layer. The winning neuron, which is that neuron
    /// with the higest output is the winner, this winning neuron is often used to
    /// classify the input into a group.
    /// </summary>
    ///
    public class SOMPattern : INeuralNetworkPattern
    {
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

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer. SOM networks do not have hidden layers, so this will
        /// throw an error.
        /// </summary>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A SOM network does not have hidden layers.");
        }

        /// <summary>
        /// Clear out any hidden neurons.
        /// </summary>
        ///
        public virtual void Clear()
        {
        }

        /// <summary>
        /// Generate the RSOM network.
        /// </summary>
        public IMLMethod Generate()
        {
            var som = new SOMNetwork(_inputNeurons, _outputNeurons);
            som.Reset();
            return som;
        }

        /// <summary>
        /// Set the activation function. A SOM uses a linear activation function, so
        /// this method throws an error.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A SOM network can't define an activation function.");
            }
        }


        /// <summary>
        /// Set the input neuron count.
        /// </summary>
        public int InputNeurons
        {
            set { _inputNeurons = value; }
        }


        /// <summary>
        /// Set the output neuron count.
        /// </summary>
        public int OutputNeurons
        {
            set { _outputNeurons = value; }
        }

        #endregion
    }
}
