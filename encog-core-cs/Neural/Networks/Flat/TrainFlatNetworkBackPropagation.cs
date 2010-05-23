using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// Train a flat network using backpropagation.
    /// </summary>
    public class TrainFlatNetworkBackPropagation:
        TrainFlatNetworkMulti
    {
        /// <summary>
        /// The learning rate.
        /// </summary>
        public double LearningRate { get; set; }

        /// <summary>
        /// The momentum.
        /// </summary>
        public double Momentum { get; set; }

        /// <summary>
        /// The last delta values.
        /// </summary>
        private double[] lastDelta;

        /// <summary>
        /// Construct a backprop trainer for flat networks.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        /// <param name="learningRate">The learning rate.</param>
        /// <param name="momentum">The momentum.</param>
        public TrainFlatNetworkBackPropagation(
            FlatNetwork network,
            INeuralDataSet training,
            double learningRate,
            double momentum):
            base(network,training)
        {
            this.Momentum = momentum;
            this.LearningRate = learningRate;
            this.lastDelta = new double[network.Weights.Length];
        }

        /// <summary>
        /// Update a weight.
        /// </summary>
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index.</param>
        /// <returns>The weight delta.</returns>
        public override double UpdateWeight(double[] gradients, double[] lastGradient, int index)
        {
            double delta = (gradients[index] * this.LearningRate) +
                (lastDelta[index] * this.Momentum);
            lastDelta[index] = delta;
            return delta;
        }
    }
}
