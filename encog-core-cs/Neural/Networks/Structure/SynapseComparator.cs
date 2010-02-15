// Encog(tm) Artificial Intelligence Framework v2.3
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
using Encog.Neural.Networks.Synapse;

namespace Encog.Neural.Networks.Structure
{
    /// <summary>
    /// Used to compare the order of synapses. Allows synapses to be sorted the same
    /// way each time. They are sorted from output back to input.
    /// </summary>
    public class SynapseComparator : IComparer<ISynapse>
    {
        /// <summary>
        /// Used to compare layers.
        /// </summary>
        private LayerComparator layerCompare;

        /// <summary>
        /// Construct a layer comparator. 
        /// </summary>
        /// <param name="structure">The structure of the network to use.</param>
        public SynapseComparator(NeuralStructure structure)
        {
            this.layerCompare = new LayerComparator(structure);
        }

        /// <summary>
        /// Compare two layers. 
        /// </summary>
        /// <param name="synapse1">The first layer to compare.</param>
        /// <param name="synapse2">The second layer to compare.</param>
        /// <returns>The value 0 if the argument layer is equal to this synapse; a
        /// value less than 0 if this synapse is less than the argument; and
        /// a value greater than 0 if this synapse is greater than the
        /// synapse argument.</returns>
        public int Compare(ISynapse synapse1, ISynapse synapse2)
        {

            if (synapse1 == synapse2)
            {
                return 0;
            }

            int cmp = this.layerCompare.Compare(synapse1.ToLayer,
                   synapse2.ToLayer);

            if (cmp != 0)
            {
                return cmp;
            }

            return this.layerCompare.Compare(synapse1.FromLayer, synapse2
                    .FromLayer);
        }

    }
}
