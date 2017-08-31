using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Propagation.SGD.Update
{
    public class RMSPropUpdate : IUpdateRule
    {
        private StochasticGradientDescent _training;
        private double[] _cache;
        public double EPS { get; set; }
        public double DecayRate { get; set; }

        public RMSPropUpdate()
        {
            DecayRate = 0.99;
            EPS = 1e-8;
        }

        public void Init(StochasticGradientDescent theTraining)
        {
            _training = theTraining;
            _cache = new double[theTraining.Flat.Weights.Length];
        }

        public void Update(double[] gradients, double[] weights)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                _cache[i] += gradients[i] * gradients[i];
                _cache[i] = DecayRate * _cache[i] + (1 - DecayRate) * gradients[i] * gradients[i];
                double delta = (_training.LearningRate * gradients[i]) / (Math.Sqrt(_cache[i]) + EPS);
                weights[i] += delta;
            }
        }
    }
}
