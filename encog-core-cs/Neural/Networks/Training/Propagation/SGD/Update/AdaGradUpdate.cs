using System;

namespace Encog.Neural.Networks.Training.Propagation.SGD.Update
{
    public class AdaGradUpdate : IUpdateRule
    {
        private StochasticGradientDescent _training;
        private double[] _cache;
        private double EPS { get; set; }

        public AdaGradUpdate()
        {
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
                double delta = (_training.LearningRate * gradients[i]) / (Math.Sqrt(_cache[i]) + EPS);
                weights[i] += delta;
            }
        }
    }
}
