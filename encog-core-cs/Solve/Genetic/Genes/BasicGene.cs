// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

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
