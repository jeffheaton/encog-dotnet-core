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
using System.Text;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// Implements a link between two NEAT neurons.
    /// 
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// -----------------------------------------------------------------------------
    /// http://www.cs.ucf.edu/~kstanley/
    /// Encog's NEAT implementation was drawn from the following three Journal
    /// Articles. For more complete BibTeX sources, see NEATNetwork.java.
    /// 
    /// Evolving Neural Networks Through Augmenting Topologies
    /// 
    /// Generating Large-Scale Neural Networks Through Discovering Geometric
    /// Regularities
    /// 
    /// Automatic feature selection in neuroevolution
    /// </summary>
    [Serializable]
    public class NEATLink : IComparable<NEATLink>
    {
        /// <summary>
        /// The source neuron.
        /// </summary>
        public int FromNeuron { get; set; }

        /// <summary>
        /// The target neuron.
        /// </summary>
        public int ToNeuron { get; set; }

        /// <summary>
        /// The weight.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Construct a NEAT link. 
        /// </summary>
        /// <param name="theFromNeuron">The from neuron.</param>
        /// <param name="theToNeuron">The to neuron.</param>
        /// <param name="theWeight">The weight.</param>
        public NEATLink(int theFromNeuron, int theToNeuron,
                double theWeight)
        {
            FromNeuron = theFromNeuron;
            ToNeuron = theToNeuron;
            Weight = theWeight;
        }

        /// <inheritdoc/>
        public int CompareTo(NEATLink other)
        {
            int result = FromNeuron - other.FromNeuron;
            if (result != 0)
            {
                return result;
            }

            return ToNeuron - other.ToNeuron;
        }

        /// <inheritdoc/>
        public override bool Equals(Object other)
        {
            if (other == null)
            {
                return false;
            }
            if (other == this)
            {
                return true;
            }
            if (!(other is NEATLink))
            {
                return false;
            }
            var otherMyClass = (NEATLink)other;
            return CompareTo(otherMyClass) == 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("[NEATLink: fromNeuron=");
            result.Append(FromNeuron);
            result.Append(", toNeuron=");
            result.Append(ToNeuron);
            result.Append("]");
            return result.ToString();
        }
    }
}
