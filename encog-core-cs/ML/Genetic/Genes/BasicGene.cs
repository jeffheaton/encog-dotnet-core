using System;

namespace Encog.ML.Genetic.Genes
{
    /// <summary>
    /// Implements the basic functionality for a gene. This is an abstract class.
    /// </summary>
    ///
    [Serializable]
    public abstract class BasicGene : IGene
    {
        /// <summary>
        /// Is this gene enabled?
        /// </summary>
        ///
        private bool enabled;

        /// <summary>
        /// ID of this gene, -1 for unassigned.
        /// </summary>
        ///
        private long id;

        /// <summary>
        /// Innovation ID, -1 for unassigned.
        /// </summary>
        ///
        private long innovationId;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public BasicGene()
        {
            enabled = true;
            id = -1;
            innovationId = -1;
        }

        #region IGene Members

        /// <inheritdoc/>
        public int CompareTo(IGene o)
        {
            return ((int) (InnovationId - o.InnovationId));
        }

        /// <summary>
        /// Set the id for this gene.
        /// </summary>
        public long Id
        {
            get { return id; }
            set { id = value; }
        }


        /// <summary>
        /// Set the innovation id for this gene.
        /// </summary>
        public long InnovationId
        {
            get { return innovationId; }
            set { innovationId = value; }
        }


        /// <value>True, if this gene is enabled.</value>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }


        /// <summary>
        /// from Encog.ml.genetic.genes.Gene
        /// </summary>
        ///
        public abstract void Copy(IGene gene);

        #endregion
    }
}