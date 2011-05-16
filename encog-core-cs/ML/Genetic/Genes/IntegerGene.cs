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
        /// Serial id.
        /// </summary>
        ///
        private const long serialVersionUID = 1L;

        /// <summary>
        /// The value of this gene.
        /// </summary>
        ///
        private int value_ren;

        /// <summary>
        /// Set the value of this gene.
        /// </summary>
        ///
        /// <value>The value of this gene.</value>
        public int Value
        {
            /// <returns>The value of this gene.</returns>
            get { return value_ren; }
            /// <summary>
            /// Set the value of this gene.
            /// </summary>
            ///
            /// <param name="theValue">The value of this gene.</param>
            set { value_ren = value; }
        }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        ///
        /// <param name="gene">The other gene to copy.</param>
        public override sealed void Copy(IGene gene)
        {
            value_ren = ((IntegerGene) gene).Value;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
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

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            return "" + value_ren;
        }
    }
}