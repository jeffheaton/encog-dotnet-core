//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
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
