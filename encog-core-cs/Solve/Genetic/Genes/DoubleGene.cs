using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Genetic.Genes
{
    /// <summary>
    /// A gene that contains a floating point value.
    /// </summary>
    public class DoubleGene : BasicGene
    {
        /// <summary>
        /// The double value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        /// <param name="gene">The other gene to copy.</param>
        public override void Copy(IGene gene)
        {
            Value = ((DoubleGene)gene).Value;

        }

        /// <summary>
        /// The gene as a string.
        /// </summary>
        /// <returns>The gene as a string.</returns>
        public override String ToString()
        {
            return "" + Value;
        }
    }
}
