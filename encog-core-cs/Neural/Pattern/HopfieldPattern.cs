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
using Encog.Neural.Thermal;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Create a Hopfield pattern. A Hopfield neural network has a single layer that
    /// functions both as the input and output layers. There are no hidden layers.
    /// Hopfield networks are used for basic pattern recognition. When a Hopfield
    /// network recognizes a pattern, it "echos" that pattern on the output.
    /// </summary>
    ///
    public class HopfieldPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// How many neurons in the Hopfield network. Default to -1, which is
        /// invalid. Therefore this value must be set.
        /// </summary>
        ///
        private int _neuronCount;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public HopfieldPattern()
        {
            _neuronCount = -1;
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer. This will throw an error, because the Hopfield neural
        /// network has no hidden layers.
        /// </summary>
        ///
        /// <param name="count">The number of neurons.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A Hopfield network has no hidden layers.");
        }

        /// <summary>
        /// Nothing to clear.
        /// </summary>
        ///
        public virtual void Clear()
        {
        }

        /// <summary>
        /// Generate the Hopfield neural network.
        /// </summary>
        ///
        /// <returns>The generated network.</returns>
        public IMLMethod Generate()
        {
            var logic = new HopfieldNetwork(_neuronCount);
            return logic;
        }

        /// <summary>
        /// Set the activation function to use. This function will throw an error,
        /// because the Hopfield network must use the BiPolar activation function.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A Hopfield network will use the BiPolar activation "
                    + "function, no activation function needs to be specified.");
            }
        }


        /// <summary>
        /// Set the number of input neurons, this must match the output neurons.
        /// </summary>
        public int InputNeurons
        {
            set { _neuronCount = value; }
        }


        /// <summary>
        /// Set the number of output neurons, should not be used with a hopfield
        /// neural network, because the number of input neurons defines the number of
        /// output neurons.
        /// </summary>
        public int OutputNeurons
        {
            set
            {
                throw new PatternError(
                    "A Hopfield network has a single layer, so no need "
                    + "to specify the output count.");
            }
        }

        #endregion
    }
}
