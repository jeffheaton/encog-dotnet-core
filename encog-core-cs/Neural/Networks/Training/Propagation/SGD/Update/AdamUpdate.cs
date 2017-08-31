using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Propagation.SGD.Update
{
    public class AdamUpdate : IUpdateRule
    {
        public void Init(StochasticGradientDescent training)
        {
            throw new NotImplementedException();
        }

        public void Update(double[] gradients, double[] weights)
        {
            throw new NotImplementedException();
        }
    }
}
