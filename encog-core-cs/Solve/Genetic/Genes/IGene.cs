using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Genetic.Genes
{
    /// <summary>
    /// Describes a gene.  A gene is the smallest piece of genetic information in Encog.
    /// </summary>
    public interface IGene: IComparable<IGene>
    {
        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        /// <param name="gene">The other gene to copy.</param>
        void Copy(IGene gene);

        /// <summary>
        /// Is this gene enabled.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Get the ID of this gene, -1 for undefined
        /// </summary>
        long Id
        {
            get;
        }

        /// <summary>
        /// The innovation ID of this gene.
        /// </summary>
        long InnovationId
        {
            get;
        }
    }
}
