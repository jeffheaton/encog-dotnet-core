using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Propagation.SGD.Update
{
    /// <summary>
    /// Defines an update rule.
    /// </summary>
    public interface IUpdateRule
    {
        void Init(StochasticGradientDescent training);
        void Update(double[] gradients, double[] weights);
    }
}
