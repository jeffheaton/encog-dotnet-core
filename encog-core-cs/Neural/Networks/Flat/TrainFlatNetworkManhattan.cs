using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training.Propagation.Manhattan;

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// Train a flat network using Manhattan prop.
    /// </summary>
    public class TrainFlatNetworkManhattan:TrainFlatNetworkMulti
    {
        /// <summary>
        /// The zero tolearnce to use.
        /// </summary>
        public double ZeroTolerance { get; set; }

        /// <summary>
        /// The learning rate.
        /// </summary>
        public double LearningRate { get; set; }

        /// <summary>
        /// Construct a trainer for flat networks to use the Manhattan update rule.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="learningRate">The learning rate to use.</param>
        public TrainFlatNetworkManhattan(
            FlatNetwork network,
            INeuralDataSet training,
            double learningRate):
            base(network,training)
        {
            LearningRate = learningRate;
            ZeroTolerance = Encog.DEFAULT_DOUBLE_EQUAL;
        }

        /// <summary>
        /// Calculate the amount to change the weight by.
        /// </summary>
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index to update.</param>
        /// <returns>The amount to change the weight by.</returns>
        public override double UpdateWeight(double[] gradients, double[] lastGradient, int index)
        {
            if (Math.Abs(gradients[index]) < this.ZeroTolerance)
            {
                return 0;
            }
            else if (gradients[index] > 0)
            {
                return this.LearningRate;
            }
            else
            {
                return -this.LearningRate;
            }       
        }
    }
}
