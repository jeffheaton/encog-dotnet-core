using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Codec;
using Encog.ML.EA.Genome;

namespace Encog.ML.Prg
{
    /// <summary>
    /// Encode and decode an Encog program between genome and phenotypes. This is a
    /// passthrough, as the Encog geneome and phenome are identical.
    /// </summary>
    public class PrgCODEC : IGeneticCODEC
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
