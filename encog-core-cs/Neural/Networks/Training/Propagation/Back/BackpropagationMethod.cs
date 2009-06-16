using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Neural.Networks.Layers;
using Encog.Matrix;
using Encog.Util.Logging;

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
        private Backpropagation propagation;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));

        /// <summary>
        /// Utility class to calculate the partial derivatives.
        /// </summary>
        private CalculatePartialDerivative pderv
            = new CalculatePartialDerivative();

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
        public void Init(Propagation propagation)
        {
            this.propagation = (Backpropagation)propagation;
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
                        double delta = level.ThresholdGradients[i]
                                * this.propagation.LearningRate;
                        delta += level.LastThresholdGradent[i]
                                * this.propagation.Momentum;
                        layer.Threshold[i] += delta;
                        level.LastThresholdGradent[i] = delta;
                        level.ThresholdGradients[i] = 0.0;
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
                   this.propagation.LearningRate);
            Matrix.Matrix m2 = MatrixMath.Multiply(synapse.LastMatrixGradients,
                   this.propagation.Momentum);
            synapse.LastMatrixGradients = MatrixMath.Add(m1, m2);

            if (this.logger.IsDebugEnabled)
            {
                this.logger.Debug("Backpropagation learning: applying delta=\n"
                        + DumpMatrix.DumpMatrixString(synapse.LastMatrixGradients));
            }
            synapse.Synapse.WeightMatrix.Add(synapse.LastMatrixGradients);
            if (this.logger.IsDebugEnabled)
            {
                this.logger.Debug("Backpropagation learning: new weight matrix=\n"
                        + DumpMatrix.DumpMatrixString(synapse.Synapse.WeightMatrix));
            }

            synapse.AccMatrixGradients.Clear();

        }

    }
}
