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

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Patterns are used to create common sorts of neural networks. Information
    /// about the structure of the neural network is communicated to the pattern, and
    /// then generate is called to produce a neural network of this type.
    /// </summary>
    ///
    public interface INeuralNetworkPattern
    {
        /// <summary>
        /// Set the activation function to be used for all created layers that allow
        /// an activation function to be specified. Not all patterns allow the
        /// activation function to be specified.
        /// </summary>
        ///
        /// <value>The activation function.</value>
        IActivationFunction ActivationFunction { 
            set; }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        ///
        /// <value>The number of input neurons.</value>
        int InputNeurons { set; }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        ///
        /// <value>The output neuron count.</value>
        int OutputNeurons {set; }

        /// <summary>
        /// Add the specified hidden layer.
        /// </summary>
        ///
        /// <param name="count">The number of neurons in the hidden layer.</param>
        void AddHiddenLayer(int count);

        /// <summary>
        /// Clear the hidden layers so that they can be redefined.
        /// </summary>
        ///
        void Clear();

        /// <summary>
        /// Generate the specified neural network.
        /// </summary>
        ///
        /// <returns>The resulting neural network.</returns>
        IMLMethod Generate();
    }
}
