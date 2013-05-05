using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Train;
using Encog.MathUtil.Randomize;

namespace Encog.Neural.NEAT.Training.Opp.Links
{
    /// <summary>
    /// Select a fixed number of link genes. If the genome does not have enough links
    /// to select the specified count, then all genes will be returned.
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
    public class SelectFixed : ISelectLinks
    {
        /// <summary>
        /// The number of links to choose.
        /// </summary>
        private int linkCount;

        /// <summary>
        /// The trainer.
        /// </summary>
        private IEvolutionaryAlgorithm trainer;

        /// <summary>
        /// Construct a fixed count link selector.
        /// </summary>
        /// <param name="theLinkCount">The number of links to select.</param>
        public SelectFixed(int theLinkCount)
        {
            this.linkCount = theLinkCount;
        }

        /// <summary>
        /// The trainer.
        /// </summary>
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
        public IList<NEATLinkGene> SelectLinks(EncogRandom rnd,
                NEATGenome genome)
        {
            IList<NEATLinkGene> result = new List<NEATLinkGene>();
            int cnt = Math.Min(this.linkCount, genome.LinksChromosome.Count);

            while (result.Count < cnt)
            {
                int idx = rnd.Next(genome.LinksChromosome.Count);
                NEATLinkGene link = genome.LinksChromosome[idx];
                if (!result.Contains(link))
                {
                    result.Add(link);
                }
            }
            return result;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            result.Append(this.GetType().Name);
            result.Append(":linkCount=");
            result.Append(this.linkCount);
            result.Append("]");
            return result.ToString();
        }
    }
}
