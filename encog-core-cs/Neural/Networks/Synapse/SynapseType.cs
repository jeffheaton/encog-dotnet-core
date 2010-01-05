// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Synapse
{
    /// <summary>
    /// Specifies the type of synapse to be created.
    /// </summary>
    public enum SynapseType
    {
        /// <summary>
        /// OneToOne - Each neuron is connected to the same neuron number
        /// in the next layer.  The two layers must have the same number
        /// of neurons.
        /// </summary>
        OneToOne,

        /// <summary>
        /// Weighted - The neurons are connected between the two levels
        /// with weights.  These weights change during training.
        /// </summary>
        Weighted,

        /// <summary>
        /// Weightless - Every neuron is connected to every other neuron
        /// in the next layer, but there are no weights.
        /// </summary>
        Weightless,

        /// <summary>
        /// Direct - Input is simply passed directly to the next layer.
        /// </summary>
        Direct,

        /// <summary>
        /// Normalize - A synapse that normalizes the data.  Used to implement
        /// a SOM.
        /// </summary>
        Normalize
    }

}
