using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.EA.Genome
{
    /// <summary>
    /// Defines a factory that produces genomes.
    /// </summary>
    public interface IGenomeFactory
    {
        /// <summary>
        /// Factor a new genome.
        /// </summary>
        /// <returns>The newly created genome.</returns>
        IGenome Factor();

        /// <summary>
        /// Create a clone of the other genome.
        /// </summary>
        /// <param name="other">The other genome.</param>
        /// <returns>The newly created clone.</returns>
        IGenome Factor(IGenome other);
    }
}
