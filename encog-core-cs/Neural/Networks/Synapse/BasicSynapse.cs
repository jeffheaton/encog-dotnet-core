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
using Encog.ML.Data;
using Encog.Neural.Networks.Layers;
using Encog.Persist;
using Encog.Neural.Data;
using Encog.MathUtil.Matrices;

#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Synapse
{
    /// <summary>
    /// An abstract class that implements basic functionality that may be needed by
    /// the other synapse classes. Specifically this class handles processing the
    /// from and to layer, as well as providing a name and description for the
    /// EncogPersistedObject.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class BasicSynapse : BasicPersistedSubObject, ISynapse
    {
        /// <summary>
        /// The from layer.
        /// </summary>
        private ILayer fromLayer;

        /// <summary>
        /// The to layer.
        /// </summary>
        private ILayer toLayer;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));
#endif


        /// <summary>
        /// The from layer.
        /// </summary>
        public ILayer FromLayer
        {
            get
            {
                return this.fromLayer;
            }
            set
            {
                this.fromLayer = value;
            }
        }

        /// <summary>
        /// The neuron count from the "from layer".
        /// </summary>
        public int FromNeuronCount
        {
            get
            {
                return this.fromLayer.NeuronCount;
            }
        }

        /// <summary>
        /// The "to layer".
        /// </summary>
        public ILayer ToLayer
        {
            get
            {
                return this.toLayer;
            }
            set
            {
                this.toLayer = value;
            }
        }

        /// <summary>
        /// The neuron count from the "to layer".
        /// </summary>
        public int ToNeuronCount
        {
            get
            {
                return this.toLayer.NeuronCount;
            }
        }

        /// <summary>
        /// True if this is a self-connected synapse. That is, the from and
        /// to layers are the same.
        /// </summary>
        public bool IsSelfConnected
        {
            get
            {
                return this.fromLayer == this.toLayer;
            }
        }

        /// <summary>
        /// What type of synapse is this?
        /// </summary>
        public abstract SynapseType SynapseType { get; }

        /// <summary>
        /// Get the size of the matrix, or zero if one is not defined.
        /// </summary>
        public abstract int MatrixSize { get; }

        /// <summary>
        /// Get the weight matrix.
        /// </summary>
        public abstract Matrix WeightMatrix { get; set; }


        /// <summary>
        /// Compute the output from this synapse.
        /// </summary>
        /// <param name="input">The input to this synapse.</param>
        /// <returns>The output from this synapse.</returns>
        public abstract MLData Compute(MLData input);

        /// <summary>
        /// True if the weights for this synapse can be modified.
        /// </summary>
        public virtual bool IsTeachable
        {
            get
            {
                return false;
            }
        }



        /// <summary>
        /// Convert this layer to a string.
        /// </summary>
        /// <returns>The layer as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[");
            result.Append(GetType().Name);
            result.Append(": from=");
            result.Append(this.FromNeuronCount);
            result.Append(",to=");
            result.Append(this.ToNeuronCount);
            result.Append("]");
            return result.ToString();
        }

    }

}
