using System;

namespace Encog.ML.Genetic.Genes
{
    /// <summary>
    /// A gene that holds a single character.
    /// </summary>
    ///
    public class CharGene : BasicGene
    {
        /// <summary>
        /// The character value of the gene.
        /// </summary>
        ///
        private char value_ren;

        /// <summary>
        /// Set the value of this gene.
        /// </summary>
        ///
        /// <value>The new value of this gene.</value>
        public char Value
        {
            get { return value_ren; }           
            set { value_ren = value; }
        }

        /// <summary>
        /// Copy another gene to this gene.
        /// </summary>
        ///
        /// <param name="gene">The source gene.</param>
        public override sealed void Copy(IGene gene)
        {
            value_ren = ((CharGene) gene).Value;
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