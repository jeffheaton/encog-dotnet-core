using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;

namespace Encog.ML.Genetic.Genome
{
    /// <summary>
    /// A factory that creates DoubleArrayGenome objects of a specific size.
    /// </summary>
    public class DoubleArrayGenomeFactory : IGenomeFactory
    {
        /// <summary>
        /// The size to create.
        /// </summary>
        private int size;

        /// <summary>
        /// Construct the genome factory.
        /// </summary>
        /// <param name="theSize">The size to create genomes of.</param>
        public DoubleArrayGenomeFactory(int theSize)
        {
            this.size = theSize;
        }

        /// <inheritdoc/>
        public IGenome Factor()
        {
            return new DoubleArrayGenome(this.size);
        }

        /// <inheritdoc/>
        public IGenome Factor(IGenome other)
        {
            // TODO Auto-generated method stub
            return new DoubleArrayGenome((DoubleArrayGenome)other);
        }
    }
}
