using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Genetic.Genes
{
    /// <summary>
    /// A gene that holds an integer.
    /// </summary>
    public class IntegerGene : BasicGene
    {
        /// <summary>
        /// The value of this gene.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        /// <param name="gene">The other gene to copy.</param>
        public override void Copy(IGene gene)
        {
            Value = ((IntegerGene)gene).Value;

        }

        /// <summary>
        /// Determine if this gene has the same values as another.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(Object obj)
        {
            if (obj is IntegerGene)
            {
                return (((IntegerGene)obj).Value == Value);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Generate a hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return this.Value;
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
