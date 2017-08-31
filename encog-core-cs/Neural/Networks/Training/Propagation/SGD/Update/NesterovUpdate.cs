using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Propagation.SGD.Update
{
    public class NesterovUpdate : IUpdateRule
    {
        private StochasticGradientDescent _training;
        private double[] _lastDelta;

        public void Init(StochasticGradientDescent theTraining)
        {
            _training = theTraining;
            _lastDelta = new double[theTraining.Flat.Weights.Length];
        }

        public void Update(double[] gradients, double[] weights)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                double prevNesterov = _lastDelta[i];
                _lastDelta[i] = (_training.Momentum * prevNesterov)
                        + (gradients[i] * _training.LearningRate);
                double delta = (_training.Momentum * prevNesterov) - ((1 + _training.Momentum) * _lastDelta[i]);
                weights[i] += delta;
            }
        }
    }
}
