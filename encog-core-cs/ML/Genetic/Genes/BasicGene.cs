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

        /// <summary>
        /// 
        /// </summary>
        ///
        public int CompareTo(IGene o)
        {
            return ((int) (InnovationId - o.InnovationId));
        }

        /// <summary>
        /// Set the id for this gene.
        /// </summary>
        ///
        /// <value>The id for this gene.</value>
        public long Id
        {
            /// <returns>The id of this gene.</returns>
            get { return id; }
            /// <summary>
            /// Set the id for this gene.
            /// </summary>
            ///
            /// <param name="i">The id for this gene.</param>
            set { id = value; }
        }


        /// <summary>
        /// Set the innovation id for this gene.
        /// </summary>
        ///
        /// <value>The innovation id for this gene.</value>
        public long InnovationId
        {
            /// <returns>The innovation id of this gene.</returns>
            get { return innovationId; }
            /// <summary>
            /// Set the innovation id for this gene.
            /// </summary>
            ///
            /// <param name="theInnovationID">The innovation id for this gene.</param>
            set { innovationId = value; }
        }


        /// <value>True, if this gene is enabled.</value>
        public bool Enabled
        {
            /// <returns>True, if this gene is enabled.</returns>
            get { return enabled; }
            /// <param name="e">True, if this gene is enabled.</param>
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