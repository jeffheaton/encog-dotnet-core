using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Population
{
    /// <summary>
    /// Generate a random population.
    /// </summary>
    public interface IPopulationGenerator
    {
        /// <summary>
        /// Generate a random genome.
        /// </summary>
        /// <param name="rnd">A random number generator.</param>
        /// <returns>A random genome.</returns>
        IGenome Generate(Random rnd);

        /// <summary>
        /// Generate a random population. 
        /// </summary>
        /// <param name="rnd">Random number generator.</param>
        /// <param name="pop">The population to generate into.</param>
        void Generate(Random rnd, IPopulation pop);
    }
}
