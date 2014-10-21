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
using Encog.Neural.BAM;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Construct a Bidirectional Access Memory (BAM) neural network. This neural
    /// network type learns to associate one pattern with another. The two patterns
    /// do not need to be of the same length. This network has two that are connected
    /// to each other. Though they are labeled as input and output layers to Encog,
    /// they are both equal, and should simply be thought of as the two layers that
    /// make up the net.
    /// </summary>
    ///
    public class BAMPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of neurons in the first layer.
        /// </summary>
        ///
        private int _f1Neurons;

        /// <summary>
        /// The number of neurons in the second layer.
        /// </summary>
        ///
        private int _f2Neurons;

        /// <summary>
        /// Set the F1 neurons. The BAM really does not have an input and output
        /// layer, so this is simply setting the number of neurons that are in the
        /// first layer.
        /// </summary>
        ///
        /// <value>The number of neurons in the first layer.</value>
        public int F1Neurons
        {
            set { _f1Neurons = value; }
        }


        /// <summary>
        /// Set the output neurons. The BAM really does not have an input and output
        /// layer, so this is simply setting the number of neurons that are in the
        /// second layer.
        /// </summary>
        public int F2Neurons
        {
            set { _f2Neurons = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Unused, a BAM has no hidden layers.
        /// </summary>
        ///
        /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A BAM network has no hidden layers.");
        }

        /// <summary>
        /// Clear any settings on the pattern.
        /// </summary>
        public void Clear()
        {
            _f1Neurons = 0;
            _f2Neurons = 0;
        }


        /// <returns>The generated network.</returns>
        public IMLMethod Generate()
        {
            var bam = new BAMNetwork(_f1Neurons, _f2Neurons);
            return bam;
        }

        /// <summary>
        /// Not used, the BAM uses a bipoloar activation function.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A BAM network can't specify a custom activation function.");
            }
        }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        public int InputNeurons
        {
            set
            {
                throw new PatternError(
                    "A BAM network has no input layer, consider setting F1 layer.");
            }
        }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        public int OutputNeurons
        {
            set
            {
                throw new PatternError(
                    "A BAM network has no output layer, consider setting F2 layer.");
            }
        }

        #endregion
    }
}
