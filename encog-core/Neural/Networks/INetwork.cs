using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data;

namespace Encog.Neural.Networks
{
    public interface INetwork
    {
        INeuralData Compute(INeuralData pattern);
    }
}
