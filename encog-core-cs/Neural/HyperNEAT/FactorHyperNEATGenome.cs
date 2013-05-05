using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NEAT;
using Encog.Neural.NEAT.Training;
using Encog.ML.EA.Genome;
using Encog.MathUtil.Randomize;

namespace Encog.Neural.HyperNEAT
{
    /// <summary>
    /// This factory is used to create NEATGenomes.
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
    public class FactorHyperNEATGenome : INEATGenomeFactory
    {
        /// <inheritdoc/>
        public IGenome Factor(IGenome other)
        {
            return new NEATGenome((NEATGenome)other);
        }

        /// <inheritdoc/>
        public NEATGenome Factor(List<NEATNeuronGene> neurons,
                List<NEATLinkGene> links, int inputCount,
                int outputCount)
        {
            return new NEATGenome(neurons, links, inputCount, outputCount);
        }

        /// <inheritdoc/>
        public NEATGenome Factor(EncogRandom rnd, NEATPopulation pop,
                int inputCount, int outputCount,
                double connectionDensity)
        {
            return new NEATGenome(rnd, pop, inputCount, outputCount,
                    connectionDensity);
        }

        /// <inheritdoc/>
        IGenome IGenomeFactory.Factor()
        {
            return new NEATGenome();
        }
    }
}
