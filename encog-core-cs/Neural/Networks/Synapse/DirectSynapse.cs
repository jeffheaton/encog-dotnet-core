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
using Encog.ML.Data;
using Encog.Neural.Networks.Layers;
using Encog.Persist;
using Encog.MathUtil.Matrices;

namespace Encog.Neural.Networks.Synapse
{
    /// <summary>
    /// A direct synapse will present the entire input array to each of the directly
    /// connected neurons in the next layer. This layer type is useful when building
    /// a radial basis neural network.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class DirectSynapse : BasicSynapse
    {


        /// <summary>
        /// Simple default constructor.
        /// </summary>
        public DirectSynapse()
        {

        }

        /// <summary>
        /// Construct a direct synapse between the two specified layers.
        /// </summary>
        /// <param name="fromLayer">The starting layer.</param>
        /// <param name="toLayer">The ending layer.</param>
        public DirectSynapse(ILayer fromLayer, ILayer toLayer)
        {
            this.FromLayer = fromLayer;
            this.ToLayer = toLayer;
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public override object Clone()
        {
            DirectSynapse result = new DirectSynapse(this.FromLayer,
                   this.ToLayer);
            return result;
        }

        /// <summary>
        /// Compute the output from this synapse.
        /// </summary>
        /// <param name="input">The input to this synapse.</param>
        /// <returns>The output is the same as the input.</returns>
        public override MLData Compute(MLData input)
        {
            return input;
        }

        /// <summary>
        /// Create a persistor for this type of synapse.
        /// </summary>
        /// <returns>A persistor.</returns>
        public override IPersistor CreatePersistor()
        {
            return null;
        }

        /// <summary>
        /// null, this synapse type has no matrix.
        /// </summary>
        public override Matrix WeightMatrix
        {
            get
            {
                return null;
            }
            set
            {
                throw new NeuralNetworkError(
                    "Can't set the matrix for a DirectSynapse");
            }
        }

        /// <summary>
        /// 0, this synapse type has no matrix.
        /// </summary>
        public override int MatrixSize
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// The type of synapse that this is.
        /// </summary>
        public override SynapseType SynapseType
        {
            get
            {
                return SynapseType.Direct;
            }
        }

        /// <summary>
        /// False, because this type of synapse is not teachable.
        /// </summary>
        public override bool IsTeachable
        {
            get
            {
                return false;
            }
        }
    }
}
