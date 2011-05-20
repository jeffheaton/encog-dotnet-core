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
using System.Text;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// Implements a link between two NEAT neurons.
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    ///
    [Serializable]
    public class NEATLink
    {
        /// <summary>
        /// The source neuron.
        /// </summary>
        ///
        private readonly NEATNeuron fromNeuron;

        /// <summary>
        /// Is this link recurrent.
        /// </summary>
        ///
        private readonly bool recurrent;

        /// <summary>
        /// The target neuron.
        /// </summary>
        ///
        private readonly NEATNeuron toNeuron;

        /// <summary>
        /// The weight between the two neurons.
        /// </summary>
        ///
        private readonly double weight;

        /// <summary>
        /// Default constructor, used mainly for persistance.
        /// </summary>
        ///
        public NEATLink()
        {
        }

        /// <summary>
        /// Construct a NEAT link.
        /// </summary>
        ///
        /// <param name="weight_0">The weight between the two neurons.</param>
        /// <param name="fromNeuron_1">The source neuron.</param>
        /// <param name="toNeuron_2">The target neuron.</param>
        /// <param name="recurrent_3">Is this a recurrent link.</param>
        public NEATLink(double weight_0, NEATNeuron fromNeuron_1,
                        NEATNeuron toNeuron_2, bool recurrent_3)
        {
            weight = weight_0;
            fromNeuron = fromNeuron_1;
            toNeuron = toNeuron_2;
            recurrent = recurrent_3;
        }


        /// <value>The source neuron.</value>
        public NEATNeuron FromNeuron
        {
            get { return fromNeuron; }
        }


        /// <value>The target neuron.</value>
        public NEATNeuron ToNeuron
        {
            get { return toNeuron; }
        }


        /// <value>The weight of the link.</value>
        public double Weight
        {
            get { return weight; }
        }


        /// <value>True if this is a recurrent link.</value>
        public bool Recurrent
        {
            get { return recurrent; }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[NEATLink: fromNeuron=");
            result.Append(FromNeuron.NeuronID);
            result.Append(", toNeuron=");
            result.Append(ToNeuron.NeuronID);
            result.Append("]");
            return result.ToString();
        }
    }
}
