using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;
using Encog.ML.EA.Codec;

namespace Encog.ML.Genetic
{
    /// <summary>
    /// A CODEC for IMLEncodable classes.
    /// </summary>
    [Serializable]
    public class MLEncodableCODEC: IGeneticCODEC
    {
        /// <inheritdoc/>
        public IMLMethod Decode(IGenome genome)
        {
            MLMethodGenome genome2 = (MLMethodGenome)genome;
            genome2.Decode();
            return genome2.Phenotype;
        }

        /// <inheritdoc/>
        public IGenome Encode(IMLMethod phenotype)
        {
            IMLEncodable phenotype2 = (IMLEncodable)phenotype;
            return new MLMethodGenome(phenotype2);
        }

    }
}
