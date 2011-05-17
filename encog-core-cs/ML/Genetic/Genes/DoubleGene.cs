using System;

namespace Encog.ML.Genetic.Genes
{
    /// <summary>
    /// A gene that contains a floating point value.
    /// </summary>
    ///
    public class DoubleGene : BasicGene
    {
        /// <summary>
        /// The value of this gene.
        /// </summary>
        ///
        private double value_ren;

        /// <summary>
        /// Set the value of the gene.
        /// </summary>
        ///
        /// <value>The gene's value.</value>
        public double Value
        {
            /// <returns>The gene value.</returns>
            get { return value_ren; }
            /// <summary>
            /// Set the value of the gene.
            /// </summary>
            ///
            /// <param name="theValue">The gene's value.</param>
            set { value_ren = value; }
        }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        ///
        /// <param name="gene">The other gene to copy.</param>
        public override sealed void Copy(IGene gene)
        {
            value_ren = ((DoubleGene) gene).Value;
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