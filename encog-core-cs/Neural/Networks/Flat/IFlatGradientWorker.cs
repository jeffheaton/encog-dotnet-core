using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Concurrency;

namespace Encog.Neural.Networks.Flat
{
    /// <summary>
    /// An interface used to define gradient workers for flat networks.
    /// </summary>
    public interface IFlatGradientWorker: IEncogTask
    {
        /// <summary>
        /// The weights for this worker.
        /// </summary>
        double[] Weights { get;  }

        /// <summary>
        /// The elapsed time for the last iteration of this worker.
        /// </summary>
        long ElapsedTime { get; }
    }
}
