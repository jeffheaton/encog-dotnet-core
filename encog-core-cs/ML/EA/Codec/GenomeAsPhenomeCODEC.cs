using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Codec
{
    /// <summary>
    /// This is a simple pass-through CODEC. This CODEC is used when the genome and
    /// phenome are the same class, and no decoding is necessary.
    /// </summary>
    public class GenomeAsPhenomeCODEC : IGeneticCODEC
    {
        /// <inheritdoc/>
        public IMLMethod Decode(IGenome genome)
        {
            return genome;
        }

        /// <inheritdoc/>
        public IGenome Encode(IMLMethod phenotype)
        {
            return (IGenome)phenotype;
        }
    }
}
