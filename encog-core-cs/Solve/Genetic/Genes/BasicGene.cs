using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Attributes;

namespace Encog.Solve.Genetic.Genes
{
    /// <summary>
    /// Implements the basic functionality for a gene. This is an abstract class.
    /// </summary>
    public abstract class BasicGene : IGene
    {
        /// <summary>
        /// Is this gene enabled?
        /// </summary>
        [@EGAttribute]
        private bool enabled;
        

        /// <summary>
        /// The gene id, -1 for unassigned.
        /// </summary>
        [@EGAttribute]
        private long id;

        /// <summary>
        /// The innovation id, -1 for unassigned.
        /// </summary>
        [@EGAttribute]
        private long innovationID;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BasicGene()
        {
            Enabled = true;
            Id = -1;
        }

        /// <summary>
        /// Compare to another gene, sort by innovation id's.
        /// </summary>
        /// <param name="o">The other object to compare to.</param>
        /// <returns>Zero if equal, or less than or greater to show order.</returns>
        public int CompareTo(IGene o)
        {
            return ((int)(InnovationId - o.InnovationId));
        }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        /// <param name="gene">The other gene to copy.</param>
        public abstract void Copy(IGene gene);

        /// <summary>
        /// Is this gene enabled.
        /// </summary>
        public bool Enabled 
        { 
            get
            {
                return this.enabled;
            }
 
            set
            {
                this.enabled = value;
            } 
        }

        /// <summary>
        /// ID of this gene, -1 for unassigned.
        /// </summary>
        public long Id
        {
            get 
            {
                return this.id;
            }
            set 
            {
                this.id = value;
            } 
        }

        /// <summary>
        /// Innovation ID, -1 for unassigned.
        /// </summary>
        public long InnovationId 
        {
            get
            {
                return this.innovationID;
            }
            set
            {
                this.innovationID = value;
            }
        }
    }
}
