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
using Encog.Neural.Networks.Synapse;
#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// The back propagation training algorithms requires training data to be stored
    /// for each of the synapses. The propagation class creates a PropagationSynapse
    /// object for each of the synapses in the neural network that it is training.
    /// </summary>
    public struct PropagationSynapse
    {

        /// <summary>
        /// Accumulate the error deltas for each weight matrix and bias value.
        /// </summary>
        public Matrix.Matrix AccMatrixGradients;

        /// <summary>
        /// Hold the previous matrix deltas so that "momentum" and other methods can
        /// be implemented. This handles both weights and thresholds.
        /// </summary>
        public Matrix.Matrix LastMatrixGradients;

        /// <summary>
        /// The actual layer that this training layer corresponds to.
        /// </summary>
        public ISynapse Synapse;

        /// <summary>
        /// The deltas that will be applied to the weight matrix in some propagation
        /// techniques.
        /// </summary>
        public Matrix.Matrix Deltas;

        /// <summary>
        /// Construct a PropagationSynapse object that corresponds to a specific
        /// synapse.
        /// </summary>
        /// <param name="synapse">The back propagation training object.</param>
        public PropagationSynapse(ISynapse synapse)
        {
            this.Synapse = synapse;
            int fromCount = synapse.FromNeuronCount;
            int toCount = synapse.ToNeuronCount;

            this.AccMatrixGradients = new Matrix.Matrix(fromCount, toCount);
            this.LastMatrixGradients = new Matrix.Matrix(fromCount, toCount);
            this.Deltas = new Matrix.Matrix(fromCount, toCount);
        }


        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[PropagationSynapse:");
            result.Append(this.Synapse.ToString());
            result.Append("]");
            return result.ToString();
        }
    }
}
