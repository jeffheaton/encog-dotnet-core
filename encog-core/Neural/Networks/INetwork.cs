using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks
{
    public interface INetwork
    {
        INeuralData Compute(INeuralData pattern);
    }
}
