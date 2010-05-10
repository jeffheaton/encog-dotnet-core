using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Concurrency;

namespace Encog.Neural.Networks.Flat
{
    public interface IFlatGradientWorker: IEncogTask
    {
        double[] Weights { get;  }
        int ElapsedTime { get; }
    }
}
