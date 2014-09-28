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
using Encog.ML.EA.Genome;
using Encog.Neural.NEAT.Training;
using Encog.MathUtil.Randomize;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// This interface defines additional methods defined to create NEAT genomes. It
    /// is an extension to the non-NEAT specific GenomeFactory.
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
    public interface INEATGenomeFactory : IGenomeFactory
    {

        /// <summary>
        /// Create a NEAT genome from a list of links and neurons.
        /// </summary>
        /// <param name="neurons">A list of neuron genes.</param>
        /// <param name="links">A list of link genes.</param>
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputCount">The output count.</param>
        /// <returns>The newly factored NEATGenome.</returns>
        NEATGenome Factor(List<NEATNeuronGene> neurons, List<NEATLinkGene> links,
                int inputCount, int outputCount);


        /// <summary>
        /// Create a new random NEAT genome.
        /// </summary>
        /// <param name="rnd">A random number generator.</param>
        /// <param name="pop">The NEAT population.</param>
        /// <param name="inputCount">The input count.</param>
        /// <param name="outputCount">The output count.</param>
        /// <param name="connectionDensity">The connection density. Specify 1.0 for fully connected.</param>
        /// <returns>The newly created NEAT genome.</returns>
        NEATGenome Factor(EncogRandom rnd, NEATPopulation pop, int inputCount,
                int outputCount, double connectionDensity);
    }
}
