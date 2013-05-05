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
        /// <param name="inputCount"><The input count./param>
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
        /// <param name="connectionDensity"><The connection density. Specify 1.0 for fully connected./param>
        /// <returns>The newly created NEAT genome.</returns>
        NEATGenome Factor(EncogRandom rnd, NEATPopulation pop, int inputCount,
                int outputCount, double connectionDensity);
    }
}
