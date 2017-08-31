using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Propagation.SGD.Update
{
    public class AdamUpdate : IUpdateRule
    {
        private StochasticGradientDescent _training;
        private double[] _m;
        private double[] _v;
        private double _beta1 = 0.9;
        private double _beta2 = 0.999;
        private double _eps = 1e-8;

        public void Init(StochasticGradientDescent theTraining)
        {
            _training = theTraining;
            _m = new double[theTraining.Flat.Weights.Length];
            _v = new double[theTraining.Flat.Weights.Length];
        }

        public void Update(double[] gradients, double[] weights)
        {
            for (int i = 0; i < weights.Length; i++)
            {

                _m[i] = (_beta1 * _m[i]) + (1 - _beta1) * gradients[i];
                _v[i] = (_beta2 * _v[i]) + (1 - _beta2) * gradients[i] * gradients[i];

                double mCorrect = _m[i] / (1 - Math.Pow(_beta1, _training.IterationNumber));
                double vCorrect = _v[i] / (1 - Math.Pow(_beta2, _training.IterationNumber));

                double delta = (_training.LearningRate * mCorrect) / (Math.Sqrt(vCorrect) + _eps);
                weights[i] += delta;
            }
        }
    }
}
