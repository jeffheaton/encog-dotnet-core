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

namespace Encog.Neural.Networks.Layers
{
    /// <summary>
    /// This interface defines all necessary methods for a neural network layer.
    /// </summary>
    ///
    public interface ILayer
    {
        /// <value>The activation function used for this layer.</value>
        IActivationFunction ActivationFunction { get; }


        /// <summary>
        /// Set the network that this layer belongs to.
        /// </summary>
        BasicNetwork Network { get; set; }


        /// <value>The neuron count.</value>
        int NeuronCount { get; }


        /// <summary>
        /// Most layer types will default this value to one. However, it is possible
        /// to use other values. This is the activation that will be passed over the
        /// bias weights to the inputs of this layer. See the Layer interface
        /// documentation for more information on how Encog handles bias values.
        /// </summary>
        double BiasActivation { get; set; }


        /// <summary>
        /// Set the activation function.
        /// </summary>
        ///
        /// <value>The activation function.</value>
        IActivationFunction Activation { set; }

        /// <returns>True if this layer has a bias.</returns>
        bool HasBias();
    }
}
