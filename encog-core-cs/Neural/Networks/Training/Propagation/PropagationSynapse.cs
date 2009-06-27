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
using log4net;

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// The back propagation training algorithms requires training data to be stored
    /// for each of the synapses. The propagation class creates a PropagationSynapse
    /// object for each of the synapses in the neural network that it is training.
    /// </summary>
    public class PropagationSynapse
    {

        /// <summary>
        /// Accumulate the error deltas for each weight matrix and bias value.
        /// </summary>
        private Matrix.Matrix accMatrixGradients;

        /// <summary>
        /// Hold the previous matrix deltas so that "momentum" and other methods can
        /// be implemented. This handles both weights and thresholds.
        /// </summary>
        private Matrix.Matrix lastMatrixGradients;

        /// <summary>
        /// The actual layer that this training layer corresponds to.
        /// </summary>
        private ISynapse synapse;

        /// <summary>
        /// The deltas that will be applied to the weight matrix in some propagation
        /// techniques.
        /// </summary>
        private Matrix.Matrix deltas;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));

        /// <summary>
        /// Construct a PropagationSynapse object that corresponds to a specific
        /// synapse.
        /// </summary>
        /// <param name="synapse">The back propagation training object.</param>
        public PropagationSynapse(ISynapse synapse)
        {
            this.synapse = synapse;
            int fromCount = synapse.FromNeuronCount;
            int toCount = synapse.ToNeuronCount;

            this.accMatrixGradients = new Matrix.Matrix(fromCount, toCount);
            this.lastMatrixGradients = new Matrix.Matrix(fromCount, toCount);
            this.deltas = new Matrix.Matrix(fromCount, toCount);
        }

        /// <summary>
        /// Accumulate a matrix delta.
        /// </summary>
        /// <param name="i1">The matrix row.</param>
        /// <param name="i2">The matrix column.</param>
        /// <param name="value">The delta value.</param>
        public void AccumulateMatrixDelta(int i1, int i2,
                 double value)
        {
            this.accMatrixGradients.Add(i1, i2, value);
        }

        /// <summary>
        /// The accumulated matrix gradients.
        /// </summary>
        public Matrix.Matrix AccMatrixGradients
        {
            get
            {
                return this.accMatrixGradients;
            }
        }

        /// <summary>
        /// The matrix deltas, these changes are applied to the matrix
        /// in some propagation techniques.
        /// </summary>
        public Matrix.Matrix Deltas
        {
            get
            {
                return this.deltas;
            }
        }

        /// <summary>
        /// The matrix gradients from the pervious iteration.
        /// </summary>
        public Matrix.Matrix LastMatrixGradients
        {
            get
            {
                return this.lastMatrixGradients;
            }
            set
            {
                this.lastMatrixGradients = value;
            }
        }

        /// <summary>
        /// Get the synapse that this object is linked with.
        /// </summary>
        public ISynapse Synapse
        {
            get
            {
                return this.synapse;
            }
        }


        /// <summary>
        /// This object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[PropagationSynapse:");
            result.Append(this.synapse.ToString());
            result.Append("]");
            return result.ToString();
        }
    }
}
