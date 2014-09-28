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
using Encog.ML.EA.Train;
using Encog.MathUtil.Randomize;

namespace Encog.Neural.NEAT.Training.Opp.Links
{
    /// <summary>
    /// This interface defines ways that NEAT links can be chosen for mutation.
    /// 
    /// -----------------------------------------------------------------------------
    /// http://www.cs.ucf.edu/~kstanley/ Encog's NEAT implementation was drawn from
    /// the following three Journal Articles. For more complete BibTeX sources, see
    /// NEATNetwork.java.
    /// 
    /// Evolving Neural Networks Through Augmenting Topologies
    /// 
    /// Generating Large-Scale Neural Networks Through Discovering Geometric
    /// Regularities
    /// 
    /// Automatic feature selection in neuroevolution
    /// </summary>
    public interface ISelectLinks
    {
        /// <summary>
        /// The trainer being used.
        /// </summary>
        IEvolutionaryAlgorithm Trainer { get; }

        /// <summary>
        /// Setup the selector.
        /// </summary>
        /// <param name="theTrainer">The trainer.</param>
        void Init(IEvolutionaryAlgorithm theTrainer);

        /// <summary>
        /// Select links from the specified genome.
        /// </summary>
        /// <param name="rnd">A random number generator.</param>
        /// <param name="genome">The genome to select from.</param>
        /// <returns>A List of link genomes.</returns>
        IList<NEATLinkGene> SelectLinks(EncogRandom rnd, NEATGenome genome);
    }
}
