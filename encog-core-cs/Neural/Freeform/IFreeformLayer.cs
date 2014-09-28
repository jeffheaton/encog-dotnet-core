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
using System.Collections.Generic;


namespace Encog.Neural.Freeform
{
    /// <summary>
    /// Defines a freeform layer. A layer is a group of similar neurons.
    /// </summary>
    public interface IFreeformLayer
    {
        /// <summary>
        /// Add a neuron to this layer.
        /// </summary>
        /// <param name="basicFreeformNeuron">The neuron to add.</param>
        void Add(IFreeformNeuron basicFreeformNeuron);

        /// <summary>
        /// The neurons in this layer.
        /// </summary>
        IList<IFreeformNeuron> Neurons { get;  }

        /// <summary>
        /// True if this layer has bias.
        /// </summary>
        bool HasBias { get; }

        /// <summary>
        /// Set the activation for the specified index.
        /// </summary>
        /// <param name="i">The index.</param>
        /// <param name="data">The data for that index.</param>
        void SetActivation(int i, double data);

        /// <summary>
        /// The size of this layer, including bias.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The size of this layer, no bias counted.
        /// </summary>
        int CountNonBias { get;  }
    }
}
