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
using Encog.Neural.Networks.Layers;
using Encog.Matrix;
using Encog.Util.Logging;

#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Training.Propagation.Back
{
    /// <summary>
    /// This class implements the specifics of how the backpropagation algorithm is
    /// used. Specifically, the partial derivatives are simply applied to the weight
    /// matrix.
    /// </summary>
    public class BackpropagationMethod : IPropagationMethod
    {

        /// <summary>
        /// The backpropagation class that owns this method.
        /// </summary>
        private PropagationUtil propagationUtil;

        	/**
	 * The learning rate.
	 */
	private  double learningRate;
	
	/**
	 * The momentum.
	 */
	private  double momentum;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));
#endif

        /// <summary>
        /// Utility class to calculate the partial derivatives.
        /// </summary>
        private CalculatePartialDerivative pderv
            = new CalculatePartialDerivative();

        /**
	 * Construct a back propagation method.
	 * @param learningRate The learning rate to use.
	 * @param momentum The momentum to use.
	 */
	public BackpropagationMethod( double learningRate,
			 double momentum) {
		this.learningRate = learningRate;
		this.momentum = momentum;
	}

        /// <summary>
        /// Calculate the error between these two levels.
        /// </summary>
        /// <param name="output"> The output to the "to level".</param>
        /// <param name="fromLevel">The from level.</param>
        /// <param name="toLevel">The target level.</param>
        public void CalculateError(NeuralOutputHolder output,
                 PropagationLevel fromLevel, PropagationLevel toLevel)
        {
            this.pderv.CalculateError(output, fromLevel, toLevel);

        }

        /// <summary>
        /// Setup this propagation method using the specified propagation class.
        /// </summary>
        /// <param name="propagation">The propagation class creating this method.</param>
        public void Init(PropagationUtil propagation)
        {
            this.propagationUtil = propagationUtil;
        }

        /// <summary>
        /// Modify the weight matrix and thresholds based on the last call to
        /// calcError.
        /// </summary>
        public void Learn()
        {
#if logging
            if (this.logger.IsDebugEnabled)
            {
                this.logger.Debug("Backpropagation learning pass");
            }
#endif

            foreach (PropagationLevel level in this.propagationUtil.Levels)
            {
                LearnLevel(level);
            }
        }


        /// <summary>
        /// Apply learning to this level.
        /// </summary>
        /// <param name="level">The level that is to learn.</param>
        private void LearnLevel(PropagationLevel level)
        {
            // teach the synapses
            foreach (PropagationSynapse synapse in level.Outgoing)
            {
                LearnSynapse(synapse);
            }

            // teach the threshold
            foreach (ILayer layer in level.Layers)
            {
                if (layer.HasThreshold)
                {
                    for (int i = 0; i < layer.NeuronCount; i++)
                    {
                        double delta = level.ThresholdGradents[i]
                                * this.learningRate;
                        delta += level.LastThresholdGradents[i]
                                * this.momentum;
                        layer.Threshold[i] += delta;
                        level.LastThresholdGradents[i] = delta;
                        level.ThresholdGradents[i] = 0.0;
                    }
                }
            }
        }

        /// <summary>
        /// Teach this synapse, based on the error that was calculated earlier.
        /// </summary>
        /// <param name="synapse">The synapse that is to learn.</param>
        private void LearnSynapse(PropagationSynapse synapse)
        {

            Matrix.Matrix m1 = MatrixMath.Multiply(synapse.AccMatrixGradients,
                   this.learningRate);
            Matrix.Matrix m2 = MatrixMath.Multiply(synapse.LastMatrixGradients,
                   this.momentum);
            synapse.LastMatrixGradients = MatrixMath.Add(m1, m2);

#if logging
            if (this.logger.IsDebugEnabled)
            {
                this.logger.Debug("Backpropagation learning: applying delta=\n"
                        + DumpMatrix.DumpMatrixString(synapse.LastMatrixGradients));
            }
#endif
            synapse.Synapse.WeightMatrix.Add(synapse.LastMatrixGradients);
#if logging
            if (this.logger.IsDebugEnabled)
            {
                this.logger.Debug("Backpropagation learning: new weight matrix=\n"
                        + DumpMatrix.DumpMatrixString(synapse.Synapse.WeightMatrix));
            }
#endif
            synapse.AccMatrixGradients.Clear();

        }

    }
}
