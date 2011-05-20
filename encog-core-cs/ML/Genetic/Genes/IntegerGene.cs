using System;

namespace Encog.ML.Genetic.Genes
{
    /// <summary>
    /// A gene that contains an integer value.
    /// </summary>
    ///
    public class IntegerGene : BasicGene
    {
        /// <summary>
        /// The value of this gene.
        /// </summary>
        ///
        private int value_ren;

        /// <summary>
        /// Set the value of this gene.
        /// </summary>
        public int Value
        {
            get { return value_ren; }
            set { value_ren = value; }
        }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        public override sealed void Copy(IGene gene)
        {
            value_ren = ((IntegerGene) gene).Value;
        }

        /// <inheritdoc/>
        public override sealed bool Equals(Object obj)
        {
            if (obj is IntegerGene)
            {
                return (((IntegerGene) obj).Value == value_ren);
            }
            else
            {
                return false;
            }
        }


        /// <returns>a hash code.</returns>
        public override sealed int GetHashCode()
        {
            return value_ren;
        }

        /// <inheritdoc/>
        public override sealed String ToString()
        {
            return "" + value_ren;
        }
    }
}