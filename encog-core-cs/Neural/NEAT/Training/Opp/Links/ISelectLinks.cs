using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
