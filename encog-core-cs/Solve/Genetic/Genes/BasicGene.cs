using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Solve.Genetic.Genes
{
    /// <summary>
    /// Implements the basic functionality for a gene. This is an abstract class.
    /// </summary>
    public abstract class BasicGene : IGene
    {
        /**
	 * Is this gene enabled?
	 */
        public bool Enabled { get; set; }

        /**
         * ID of this gene, -1 for unassigned.
         */
        public long Id { get; set; }

        /**
         * Innovation ID, -1 for unassigned.
         */
        public long InnovationId { get; set; }

        public BasicGene()
        {
            Enabled = true;
            Id = -1;
        }

        /**
         * Compare to another gene, sort by innovation id's.
         */
        public int CompareTo(IGene o)
        {
            return ((int)(InnovationId - o.InnovationId));
        }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        /// <param name="gene">The other gene to copy.</param>
        public abstract void Copy(IGene gene);
    }
}
