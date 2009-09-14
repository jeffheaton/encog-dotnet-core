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
#if logging
using log4net;
#endif
namespace Encog.Neural.Networks.Training.Propagation.Manhattan
{
    /// <summary>
    /// Implements the specifics of the Manhattan propagation algorithm. This class
    /// actually handles the updates to the weight matrix.
    /// </summary>
    public class ManhattanPropagationMethod : IPropagationMethod
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));
#endif
        /// <summary>
        /// The Manhattan propagation class that this method is used by.
        /// </summary>
        private ManhattanPropagation propagation;

        /// <summary>
        /// The partial derivative utility class.
        /// </summary>
        private CalculatePartialDerivative pderv
            = new CalculatePartialDerivative();

        /// <summary>
        /// Calculate the error between these two levels.
        /// </summary>
        /// <param name="output">The output to the "to level".</param>
        /// <param name="fromLevel">The from level.</param>
        /// <param name="toLevel">The target level.</param>
        public void CalculateError(NeuralOutputHolder output,
                 PropagationLevel fromLevel, PropagationLevel toLevel)
        {
            this.pderv.CalculateError(output, fromLevel, toLevel);
        }

        /**
         * Determine the change that should be applied.  If the partial
         * derivative was zero(or close enough to zero) then do nothing
         * otherwise apply the learning rate with the same sign as the
         * partial derivative.
         * @param value The partial derivative.
         * @return The change to be applied to the weight matrix.
         */
        private double DetermineChange(double value)
        {
            if (Math.Abs(value) < this.propagation.ZeroTolerance)
            {
                return 0;
            }
            else if (value > 0)
            {
                return this.propagation.LearningRate;
            }
            else
            {
                return -this.propagation.LearningRate;
            }
        }

        /// <summary>
        /// Init with the specified propagation object.
        /// </summary>
        /// <param name="propagation">The propagation object that this method will be used with.</param>
        public void Init(Propagation propagation)
        {
            this.propagation = (ManhattanPropagation)propagation;

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
            foreach (PropagationLevel level in this.propagation.Levels)
            {
                LearnLevel(level);
            }
        }


        /// <summary>
        /// Apply learning for this level.  This is where the weight matrixes
        /// are actually changed. This method will call learnSynapse for each
        /// of the synapses on this level.
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
                        double change = DetermineChange(level
                               .ThresholdGradents[i]
                               * this.propagation.LearningRate);
                        layer.Threshold[i] += change;
                    }
                }
            }
        }

        /// <summary>
        /// Learn from the last error calculation.
        /// </summary>
        /// <param name="synapse">The synapse that is to learn.</param>
        private void LearnSynapse(PropagationSynapse synapse)
        {

            Matrix.Matrix matrix = synapse.Synapse.WeightMatrix;

            for (int row = 0; row < matrix.Rows; row++)
            {
                for (int col = 0; col < matrix.Cols; col++)
                {
                    double change = DetermineChange(synapse
                           .AccMatrixGradients[row, col]);
                    matrix[row, col] += change;
                }
            }
        }

    }

}
