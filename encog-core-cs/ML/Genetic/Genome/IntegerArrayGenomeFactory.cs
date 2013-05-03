using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;

namespace Encog.ML.Genetic.Genome
{
    /// <summary>
    /// A factory to create integer genomes of a specific size.
    /// </summary>
    public class IntegerArrayGenomeFactory : IGenomeFactory
    {
        /// <summary>
        /// The size of genome to create.
        /// </summary>
        private int size;

        /// <summary>
        /// Create the integer genome of a fixed size.
        /// </summary>
        /// <param name="theSize">The size to use.</param>
        public IntegerArrayGenomeFactory(int theSize)
        {
            this.size = theSize;
        }

        /// <inheritdoc/>
        public IGenome Factor()
        {
            return new IntegerArrayGenome(this.size);
        }

        /// <inheritdoc/>
        public IGenome Factor(IGenome other)
        {
            return new IntegerArrayGenome(((IntegerArrayGenome)other));
        }
    }
}
