using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Species;
using Encog.ML.EA.Genome;

namespace Encog.ML.Prg.Species
{
    /// <summary>
    /// Perform speciation for two Encog programs. This is a threshold based
    /// speciation, similar to that used for NEAT. Any genomes with a compatibility
    /// score below a specified threshold will be in the same species.
    /// </summary>
    public class PrgSpeciation : ThresholdSpeciation
    {
        public PrgSpeciation()
        {
            CompatibilityThreshold = 15;
            MaxNumberOfSpecies = 30;
            NumGensAllowedNoImprovement = 15;
        }

        /// <inheritdoc/>
        public override double GetCompatibilityScore(IGenome genome1,
                IGenome genome2)
        {
            CompareEncogProgram comp = new CompareEncogProgram();
            double d = comp.Compare((EncogProgram)genome1,
                    (EncogProgram)genome2);
            return d;
        }
    }
}
