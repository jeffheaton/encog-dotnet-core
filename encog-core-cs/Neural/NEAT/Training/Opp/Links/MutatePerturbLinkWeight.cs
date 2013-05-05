using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Train;
using Encog.MathUtil.Randomize;

namespace Encog.Neural.NEAT.Training.Opp.Links
{
    /// <summary>
    /// Mutate weight links by perturbing their weights. This will be done by adding
    /// a Gaussian random number with the specified sigma. The sigma specifies the
    /// standard deviation of the random number. Because the random numbers are
    /// clustered at zero, this can be either an increase or decrease.
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
    public class MutatePerturbLinkWeight : IMutateLinkWeight
    {
        /// <summary>
        /// The trainer being used.
        /// </summary>
        private IEvolutionaryAlgorithm trainer;

        /// <summary>
        /// The sigma (standard deviation) of the Gaussian random numbers.
        /// </summary>
        private double sigma;

        /// <summary>
        /// Construct the perturbing mutator.
        /// </summary>
        /// <param name="theSigma">The sigma (standard deviation) for all random numbers.</param>
        public MutatePerturbLinkWeight(double theSigma)
        {
            this.sigma = theSigma;
        }

        /// <inheritdoc/>
        public IEvolutionaryAlgorithm Trainer
        {
            get
            {
                return this.trainer;
            }
        }

        /// <inheritdoc/>
        public void Init(IEvolutionaryAlgorithm theTrainer)
        {
            this.trainer = theTrainer;
        }

        /// <inheritdoc/>
        public void MutateWeight(EncogRandom rnd, NEATLinkGene linkGene,
                double weightRange)
        {
            double delta = rnd.NextGaussian() * this.sigma;
            double w = linkGene.Weight + delta;
            w = NEATPopulation.ClampWeight(w, weightRange);
            linkGene.Weight = w;
        }

        /// <inheritdoc/>
        public string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            result.Append(this.GetType().Name);
            result.Append(":sigma=");
            result.Append(this.sigma);
            result.Append("]");
            return result.ToString();
        }
    }
}
