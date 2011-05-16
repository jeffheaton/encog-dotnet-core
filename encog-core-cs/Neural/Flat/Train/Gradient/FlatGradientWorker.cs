using Encog.Util.Concurrency;

namespace Encog.Neural.Flat.Train.Gradient
{
    /// <summary>
    /// An interface used to define gradient workers for flat networks.
    /// </summary>
    ///
    public interface FlatGradientWorker : IEngineTask
    {
        /// <value>The weights for this worker.</value>
        double[] Weights { /// <returns>The weights for this worker.</returns>
            get; }


        /// <value>The network being trained by this thread.</value>
        FlatNetwork Network { /// <returns>The network being trained by this thread.</returns>
            get; }
    }
}