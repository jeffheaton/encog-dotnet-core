using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Flat
{
    public class TrainFlatNetworkBackPropagation:
        TrainFlatNetworkMulti
    {
        public double LearningRate { get; set; }
        public double Momentum { get; set; }
        private double[] lastDelta;

        public TrainFlatNetworkBackPropagation(
            FlatNetwork network,
            INeuralDataSet training,
            double learningRate,
            double momentum,
            double enforcedCLRatio):
            base(network,training,enforcedCLRatio)
        {
            this.Momentum = momentum;
            this.LearningRate = learningRate;
            this.lastDelta = new double[network.Weights.Length];
        }


        public override double UpdateWeight(double[] gradients, double[] lastGradient, int index)
        {
            double delta = (gradients[index] * this.LearningRate) +
                (lastDelta[index] * this.Momentum);
            lastDelta[index] = delta;
            return delta;
        }
    }
}
