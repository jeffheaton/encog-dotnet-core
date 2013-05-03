using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Genetic.Genome;

namespace Encog.ML.Genetic
{
    /// <summary>
    /// Implements a genome that allows a feedforward neural network to be trained
    /// using a genetic algorithm. The chromosome for a feed forward neural network
    /// is the weight and bias matrix.
    /// </summary>
    public class MLMethodGenome : DoubleArrayGenome
    {
        /// <summary>
        /// The phenome.
        /// </summary>
        public IMLEncodable Phenotype { get; set; }

        /// <summary>
        /// Construct a neural genome.
        /// </summary>
        /// <param name="thePhenotype">The phenotype to use.</param>
        public MLMethodGenome(IMLEncodable thePhenotype)
            : base(thePhenotype.EncodedArrayLength())
        {
            Phenotype = thePhenotype;
            Phenotype.EncodeToArray(Data);
        }

        /// <summary>
        /// Decode the phenotype.
        /// </summary>
        public void Decode()
        {
            Phenotype.DecodeFromArray(Data);
        }


    }
}
