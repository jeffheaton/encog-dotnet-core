// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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
using Encog.Persist;
using Encog.Neural.Data;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Networks.Synapse
{
    /// <summary>
    /// A synapse is the connection between two layers of a neural network. The
    /// various synapse types define how layers will interact with each other. Some
    /// synapses contain a weight matrix, which cause them to be teachable. Others
    /// simply feed the data between layers in various ways, and are not teachable.
    /// </summary>
    public interface ISynapse : IEncogPersistedObject
    {
        /// <summary>
        /// Compute the output from this synapse.
        /// </summary>
        /// <param name="input">The input to this synapse.</param>
        /// <returns>The output from this synapse.</returns>
        INeuralData Compute(INeuralData input);


        /// <summary>
        /// The from layer.
        /// </summary>
        ILayer FromLayer
        {
            get;
            set;
        }

        /// <summary>
        /// The neuron count from the "from layer".
        /// </summary>
        int FromNeuronCount
        {
            get;
        }

        /// <summary>
        /// Get the weight matrix.
        /// </summary>
        Matrix.Matrix WeightMatrix
        {
            get;
            set;
        }

        /// <summary>
        /// Get the size of the matrix, or zero if one is not defined.
        /// </summary>
        int MatrixSize
        {
            get;
        }

        /// <summary>
        /// The "to layer".
        /// </summary>
        ILayer ToLayer
        {
            get;
            set;
        }

        /// <summary>
        /// The neuron count from the "to layer".
        /// </summary>
        int ToNeuronCount
        {
            get;
        }

        /// <summary>
        /// The type of synapse that this is.
        /// </summary>
        SynapseType SynapseType
        {
            get;
        }

        /// <summary>
        /// True if this is a self-connected synapse.  That is,
        /// the from and to layers are the same.
        /// </summary>
        bool IsSelfConnected
        {
            get;
        }

        /// <summary>
        /// True if the weights for this synapse can be modified.
        /// </summary>
        bool IsTeachable
        {
            get;
        }
    }

}
