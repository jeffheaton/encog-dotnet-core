using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Train;
using Encog.MathUtil.Randomize;

namespace Encog.Neural.NEAT.Training.Opp.Links
{
    /// <summary>
    /// Select a random proportion of links to mutate.
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
    public class SelectProportion : ISelectLinks
    {
        /// <summary>
        /// The portion of the links to mutate. 0.0 for none, 1.0 for all.
        /// </summary>
        private double proportion;

        /// <summary>
        /// The trainer.
        /// </summary>
        private IEvolutionaryAlgorithm trainer;

        /// <summary>
        /// Select based on proportion. 
        /// </summary>
        /// <param name="theProportion">The proportion to select from.</param>
        public SelectProportion(double theProportion)
        {
            this.proportion = theProportion;
        }

        /// <inheritdoc/>
        public void Init(IEvolutionaryAlgorithm theTrainer)
        {
            this.trainer = theTrainer;
        }

        /// <inheritdoc/>
        public IList<NEATLinkGene> SelectLinks(EncogRandom rnd, NEATGenome genome)
        {
            IList<NEATLinkGene> result = new List<NEATLinkGene>();

            bool mutated = false;

            foreach (NEATLinkGene linkGene in genome.LinksChromosome)
            {
                if (rnd.NextDouble() < this.proportion)
                {
                    mutated = true;
                    result.Add(linkGene);
                }
            }

            if (!mutated)
            {
                int idx = rnd.Next(genome.LinksChromosome.Count);
                NEATLinkGene linkGene = genome.LinksChromosome[idx];
                result.Add(linkGene);
            }

            return result;
        }

        /// <inheritdoc/>
        public IEvolutionaryAlgorithm Trainer
        {
            get
            {
                return trainer;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            result.Append(this.GetType().Name);
            result.Append(":proportion=");
            result.Append(this.proportion);
            result.Append("]");
            return result.ToString();
        }
    }
}
