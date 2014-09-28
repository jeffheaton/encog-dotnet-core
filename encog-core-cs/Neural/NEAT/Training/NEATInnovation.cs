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

namespace Encog.Neural.NEAT.Training
{
    /// <summary>
    /// Implements a NEAT innovation. This lets NEAT track what changes it has
    /// previously tried with a neural network.
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
    public class NEATInnovation
    {
        /// <summary>
        /// The neuron id.
        /// </summary>
        public long NeuronId { get; set; }

        /// <summary>
        /// The innovation id.
        /// </summary>
        public long InnovationId { get; set; }


        /// <inheritdoc/>
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("[NeatInnovation:");
            result.Append("id=");
            result.Append(InnovationId);
            result.Append(",neuron=");
            result.Append(NeuronId);
            result.Append("]");
            return result.ToString();
        }

    }
}
