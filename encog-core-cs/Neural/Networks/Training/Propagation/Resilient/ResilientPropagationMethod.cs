using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Networks.Training.Propagation.Resilient
{
    /// <summary>
    /// Implements the specifics of the resilient propagation training algorithm.
    /// </summary>
    public class ResilientPropagationMethod : IPropagationMethod
    {

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));

        /// <summary>
        /// The propagation class that this method is used with.
        /// </summary>
        private ResilientPropagation propagation;

        /// <summary>
        /// Utility class to calculate the partial derivative.
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

        /// <summary>
        /// Init with the specified propagation object.
        /// </summary>
        /// <param name="propagation">The propagation object that this method will be used with.</param>
        public void Init(Propagation propagation)
        {
            this.propagation = (ResilientPropagation)propagation;

        }

        /// <summary>
        /// Modify the weight matrix and thresholds based on the last call to
        /// calcError.
        /// </summary>
        public void Learn()
        {
            if (this.logger.IsDebugEnabled)
            {
                this.logger.Debug("Backpropagation learning pass");
            }

            foreach (PropagationLevel level in this.propagation.Levels)
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
                        int change = Sign(level.ThresholdGradients[i]
                               * level.LastThresholdGradent[i]);
                        double weightChange = 0;

                        // if the gradient has retained its sign, then we increase
                        // the delta so that it will converge faster
                        if (change > 0)
                        {
                            double delta = level.ThresholdDelta[i]
                                    * ResilientPropagation.POSITIVE_ETA;
                            delta = Math.Min(delta, this.propagation.MaxStep);
                            weightChange = Sign(level.ThresholdGradients[i])
                                    * delta;
                            level.ThresholdDelta[i] = delta;
                            level.LastThresholdGradent[i] = level
                                    .ThresholdGradients[i];
                        }
                        else if (change < 0)
                        {
                            // if change<0, then the sign has changed, and the last
                            // delta was too big
                            double delta = level.ThresholdDelta[i]
                                    * ResilientPropagation.NEGATIVE_ETA;
                            delta = Math.Max(delta, ResilientPropagation.DELTA_MIN);
                            level.ThresholdDelta[i] = delta;
                            // set the previous gradient to zero so that there will
                            // be no adjustment the next iteration
                            level.LastThresholdGradent[i] = 0;
                        }
                        else if (change == 0)
                        {
                            // if change==0 then there is no change to the delta
                            double delta = level.ThresholdDelta[i];
                            weightChange = Sign(level.ThresholdGradients[i])
                                    * delta;
                            level.LastThresholdGradent[i] = level
                                    .ThresholdGradients[i];
                        }

                        // apply the weight change, if any
                        layer.Threshold[i] = layer.Threshold[i] + weightChange;

                        level.ThresholdGradients[i] = 0.0;
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

            for (int row = 0; row < matrix.Rows; row++)
            {
                for (int col = 0; col < matrix.Cols; col++)
                {
                    // multiply the current and previous gradient, and take the
                    // sign. We want to see if the gradient has changed its sign.
                    int change = Sign(synapse.AccMatrixGradients[row, col]
                           * synapse.LastMatrixGradients[row, col]);
                    double weightChange = 0;

                    // if the gradient has retained its sign, then we increase the
                    // delta so that it will converge faster
                    if (change > 0)
                    {
                        double delta = synapse.Deltas[row, col]
                                * ResilientPropagation.POSITIVE_ETA;
                        delta = Math.Min(delta, this.propagation.MaxStep);
                        weightChange = Sign(synapse.AccMatrixGradients[
                                row, col])
                                * delta;
                        synapse.Deltas[row, col] = delta;
                        synapse.LastMatrixGradients[row, col] =
                                synapse.AccMatrixGradients[row, col];
                    }
                    else if (change < 0)
                    {
                        // if change<0, then the sign has changed, and the last 
                        // delta was too big
                        double delta = synapse.Deltas[row, col]
                                * ResilientPropagation.NEGATIVE_ETA;
                        delta = Math.Max(delta, ResilientPropagation.DELTA_MIN);
                        synapse.Deltas[row, col] = delta;
                        // set the previous gradent to zero so that there will be no
                        // adjustment the next iteration
                        synapse.LastMatrixGradients[row, col] = 0;
                    }
                    else if (change == 0)
                    {
                        // if change==0 then there is no change to the delta
                        double delta = synapse.Deltas[row, col];
                        weightChange = Sign(synapse.AccMatrixGradients[row, col])
                                * delta;
                        synapse.LastMatrixGradients[row, col] =
                                synapse.AccMatrixGradients[row, col];
                    }

                    // apply the weight change, if any
                    matrix[row, col] = synapse.Synapse.WeightMatrix[row, col]
                            + weightChange;

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
            if (Math.Abs(value) < this.propagation.ZeroTolerance)
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
