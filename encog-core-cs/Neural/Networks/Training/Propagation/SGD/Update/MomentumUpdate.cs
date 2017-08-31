using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Propagation.SGD.Update
{
    public class MomentumUpdate: IUpdateRule
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
                double delta = (_training.LearningRate * gradients[i]) + (_training.Momentum * _lastDelta[i]);
                weights[i] += delta;
                _lastDelta[i] = delta;
            }
        }
    }
}
