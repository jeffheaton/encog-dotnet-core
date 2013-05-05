using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Train;
using Encog.MathUtil.Randomize;

namespace Encog.Neural.NEAT.Training.Opp.Links
{
    /// <summary>
    /// Mutate weight links by reseting the weight to an entirely new value. The
    /// weight range will come from the trainer.
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
    public class MutateResetLinkWeight : IMutateLinkWeight
    {
        /// <summary>
        /// The trainer being used.
        /// </summary>
        private IEvolutionaryAlgorithm trainer;

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
        public void MutateWeight(EncogRandom rnd, NEATLinkGene linkGene, double weightRange)
        {
            linkGene.Weight = RangeRandomizer.Randomize(rnd, -weightRange,
                    weightRange);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            result.Append(this.GetType().Name);
            result.Append("]");
            return result.ToString();
        }
    }
}
