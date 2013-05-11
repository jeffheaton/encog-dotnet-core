using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;

namespace Encog.ML.Prg.Train
{
    /// <summary>
    /// A GenomeFactory that creates EncogProgram genomes.
    /// </summary>
    [Serializable]
    public class PrgGenomeFactory : IGenomeFactory
    {
        /// <summary>
        /// The context.
        /// </summary>
        private EncogProgramContext context;

        /// <summary>
        /// Construct a factory.
        /// </summary>
        /// <param name="theContext">The context to use.</param>
        public PrgGenomeFactory(EncogProgramContext theContext)
        {
            this.context = theContext;
        }

        /// <inheritdoc/>
        public IGenome Factor()
        {
            EncogProgram result = new EncogProgram(this.context,
                    new EncogProgramVariables());
            return result;
        }

        /// <inheritdoc/>
        public IGenome Factor(IGenome other)
        {
            EncogProgram result = new EncogProgram(this.context,
                    new EncogProgramVariables());
            result.Copy(other);
            return result;
        }
    }
}
