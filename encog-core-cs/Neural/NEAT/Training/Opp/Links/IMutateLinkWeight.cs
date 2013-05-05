using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Train;
using Encog.MathUtil.Randomize;

namespace Encog.Neural.NEAT.Training.Opp.Links
{
    /// <summary>
    /// This interface defines various ways that a NEAT network can have its link
    /// weights mutated.
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
    public interface IMutateLinkWeight
    {
        /// <summary>
        /// The training class that this mutator is being used with.
        /// </summary>
        IEvolutionaryAlgorithm Trainer { get; }

        /// <summary>
        /// Setup the link mutator.
        /// </summary>
        /// <param name="theTrainer">The training class that this mutator is used with.</param>
        void Init(IEvolutionaryAlgorithm theTrainer);

        /// <summary>
        /// Perform the weight mutation on the specified link.
        /// </summary>
        /// <param name="rnd">A random number generator.</param>
        /// <param name="linkGene">The link to mutate.</param>
        /// <param name="weightRange">The weight range, weights are between -weightRange and
        /// +weightRange.</param>
        void MutateWeight(EncogRandom rnd, NEATLinkGene linkGene, double weightRange);
    }
}
