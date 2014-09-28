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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Encog.Neural.Freeform.Basic
{
    /// <summary>
    /// Implements a basic freeform layer.
    /// </summary>
    [Serializable]
    public class BasicFreeformLayer : IFreeformLayer
    {
        /// <summary>
        /// The neurons in this layer.
        /// </summary>
        private readonly IList<IFreeformNeuron> _neurons = new List<IFreeformNeuron>();

        /// <inheritdoc/>
        public void Add(IFreeformNeuron neuron)
        {
            _neurons.Add(neuron);
        }

        /// <inheritdoc/>
        public IList<IFreeformNeuron> Neurons
        {
            get
            {
                return _neurons;
            }
        }

        /// <inheritdoc/>
        public void SetActivation(int i, double activation)
        {
            _neurons[i].Activation = activation;
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                return _neurons.Count;
            }
        }

        /// <inheritdoc/>
        public int CountNonBias
        {
            get
            {
                return _neurons.Count(neuron => !neuron.IsBias);
            }
        }


        /// <inheritdoc/>
        public bool HasBias
        {
            get
            { return Neurons.Any(n => n.IsBias); }
        }
    }
}
