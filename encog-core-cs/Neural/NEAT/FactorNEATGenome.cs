using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;
using Encog.Neural.NEAT.Training;
using Encog.MathUtil.Randomize;

namespace Encog.Neural.NEAT
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
    public class FactorNEATGenome : INEATGenomeFactory
    {
        /// <inheritdoc/>
        public IGenome Factor()
        {
            return new NEATGenome();
        }

        /// <inheritdoc/>
        public IGenome Factor(IGenome other)
        {
            return new NEATGenome((NEATGenome)other);
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
        NEATGenome INEATGenomeFactory.Factor(List<NEATNeuronGene> neurons, List<NEATLinkGene> links, int inputCount, int outputCount)
        {
            return new NEATGenome(neurons, links, inputCount, outputCount);
        }
    }
}
