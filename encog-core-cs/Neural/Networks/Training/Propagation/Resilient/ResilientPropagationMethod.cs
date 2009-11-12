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

namespace Encog.Neural.Networks.Training.Propagation.Resilient
{
    /// <summary>
    /// Implements the specifics of the resilient propagation training algorithm.
    /// </summary>
    public class ResilientPropagationMethod : IPropagationMethod
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));
#endif

        /// <summary>
        /// The zero tolerance.
        /// </summary>
        private double zeroTolerance;

        /// <summary>
        /// The maximum step a delta can take.
        /// </summary>
        private double maxStep;

        /// <summary>
        /// The intial values for the deltas.
        /// </summary>
        private double initialUpdate;

        /// <summary>
        /// The propagation class that this method is used with.
        /// </summary>
        private PropagationUtil propagationUtil;

        /// <summary>
        /// Utility class to calculate the partial derivative.
        /// </summary>
        private CalculatePartialDerivative pderv
            = new CalculatePartialDerivative();


        /// <summary>
        /// Construct a resilient propagation method.
        /// </summary>
        /// <param name="zeroTolerance">The zero tolerance.</param>
        /// <param name="maxStep">The max step.</param>
        /// <param name="initialUpdate">The initial update.</param>
        public ResilientPropagationMethod(double zeroTolerance,
                 double maxStep, double initialUpdate)
        {
            this.zeroTolerance = zeroTolerance;
            this.maxStep = maxStep;
            this.initialUpdate = initialUpdate;
        }

        /// <summary>
        /// Init with the specified propagation object.
        /// </summary>
        /// <param name="propagationUtil">The propagation object that this method will be used with.</param>
        public void Init(PropagationUtil propagationUtil)
        {
            this.propagationUtil = propagationUtil;

            // set the initialUpdate to all of the threshold and matrix update
            // values.
            // This is necessary for the first step. RPROP always builds on the
            // previous
            // step, and there is no previous step on the first iteration.
            foreach (PropagationLevel level in propagationUtil.Levels)
            {
                for (int i = 0; i < level.NeuronCount; i++)
                {
                    level.ThresholdDeltas[i] = this.initialUpdate;
                }

                foreach (PropagationSynapse synapse in level.Outgoing)
                {
                    synapse.Deltas.Set(this.initialUpdate);
                }
            }
        }


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
        /// Apply the learning to the specified level.
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

                        // multiply the current and previous gradient, and take the
                        // sign. We want to see if the gradient has changed its
                        // sign.
                        int change = Sign(level.ThresholdGradents[i]
                               * level.LastThresholdGradents[i]);
                        double weightChange = 0;

                        // if the gradient has retained its sign, then we increase
                        // the delta so that it will converge faster
                        if (change > 0)
                        {
                            double delta = level.ThresholdDeltas[i]
                                    * ResilientPropagation.POSITIVE_ETA;
                            delta = Math.Min(delta, this.maxStep);
                            weightChange = Sign(level.ThresholdGradents[i])
                                    * delta;
                            level.ThresholdDeltas[i] = delta;
                            level.LastThresholdGradents[i] = level
                                    .ThresholdGradents[i];
                        }
                        else if (change < 0)
                        {
                            // if change<0, then the sign has changed, and the last
                            // delta was too big
                            double delta = level.ThresholdDeltas[i]
                                    * ResilientPropagation.NEGATIVE_ETA;
                            delta = Math.Max(delta, ResilientPropagation.DELTA_MIN);
                            level.ThresholdDeltas[i] = delta;
                            // set the previous gradient to zero so that there will
                            // be no adjustment the next iteration
                            level.LastThresholdGradents[i] = 0;
                        }
                        else if (change == 0)
                        {
                            // if change==0 then there is no change to the delta
                            double delta = level.ThresholdDeltas[i];
                            weightChange = Sign(level.ThresholdGradents[i])
                                    * delta;
                            level.LastThresholdGradents[i] = level
                                    .ThresholdGradents[i];
                        }

                        // apply the weight change, if any
                        layer.Threshold[i] = layer.Threshold[i] + weightChange;

                        level.ThresholdGradents[i] = 0.0;
                    }
                }
            }

        }

        /// <summary>
        /// Learn from the last error calculation.
        /// </summary>
        /// <param name="synapse">The synapse to teach.</param>
        private void LearnSynapse(PropagationSynapse synapse)
        {

            Matrix.Matrix matrix = synapse.Synapse.WeightMatrix;
            double[][] accData = synapse.AccMatrixGradients.Data;
            double[][] lastData = synapse.LastMatrixGradients.Data;
            double[][] deltas = synapse.Deltas.Data;
            double[][] matrixData = matrix.Data;

            for (int row = 0; row < matrix.Rows; row++)
            {
                for (int col = 0; col < matrix.Cols; col++)
                {
                    // multiply the current and previous gradient, and take the
                    // sign. We want to see if the gradient has changed its sign.
                    int change = Sign(accData[row][col]
                           * lastData[row][col]);
                    double weightChange = 0;

                    // if the gradient has retained its sign, then we increase the
                    // delta so that it will converge faster
                    if (change > 0)
                    {
                        double delta = deltas[row][col]
                                * ResilientPropagation.POSITIVE_ETA;
                        delta = Math.Min(delta, this.maxStep);
                        weightChange = Sign(accData[
                                row][col])
                                * delta;
                        deltas[row][col] = delta;
                        lastData[row][col] =
                                accData[row][col];
                    }
                    else if (change < 0)
                    {
                        // if change<0, then the sign has changed, and the last 
                        // delta was too big
                        double delta = deltas[row][col]
                                * ResilientPropagation.NEGATIVE_ETA;
                        delta = Math.Max(delta, ResilientPropagation.DELTA_MIN);
                        deltas[row][col] = delta;
                        // set the previous gradent to zero so that there will be no
                        // adjustment the next iteration
                        lastData[row][col] = 0;
                    }
                    else if (change == 0)
                    {
                        // if change==0 then there is no change to the delta
                        double delta = deltas[row][col];
                        weightChange = Sign(accData[row][col])
                                * delta;
                        lastData[row][col] =
                                accData[row][col];
                    }

                    // apply the weight change, if any
                    matrixData[row][col] += weightChange;

                }
            }

            // clear out the gradient accumulator for the next iteration
            synapse.AccMatrixGradients.Clear();
        }

        /// <summary>
        /// Determine the sign of the value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>-1 if less than zero, 1 if greater, or 0 if zero.</returns>
        private int Sign(double value)
        {
            if (Math.Abs(value) < this.zeroTolerance)
            {
                return 0;
            }
            else if (value > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

    }

}
