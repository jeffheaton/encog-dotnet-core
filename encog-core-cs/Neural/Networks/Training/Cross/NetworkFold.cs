using Encog.Neural.Flat;
using Encog.Util;

namespace Encog.Neural.Networks.Training.Cross
{
    /// <summary>
    /// The network for one fold of a cross validation.
    /// </summary>
    ///
    public class NetworkFold
    {
        /// <summary>
        /// The output for this fold.
        /// </summary>
        ///
        private readonly double[] output;

        /// <summary>
        /// The weights for this fold.
        /// </summary>
        ///
        private readonly double[] weights;

        /// <summary>
        /// Construct a fold from the specified flat network.
        /// </summary>
        ///
        /// <param name="flat">THe flat network.</param>
        public NetworkFold(FlatNetwork flat)
        {
            weights = EngineArray.ArrayCopy(flat.Weights);
            output = EngineArray.ArrayCopy(flat.LayerOutput);
        }


        /// <value>The network weights.</value>
        public double[] Weights
        {
            /// <returns>The network weights.</returns>
            get { return weights; }
        }


        /// <value>The network output.</value>
        public double[] Output
        {
            /// <returns>The network output.</returns>
            get { return output; }
        }

        /// <summary>
        /// Copy weights and output to the network.
        /// </summary>
        ///
        /// <param name="target">The network to copy to.</param>
        public void CopyToNetwork(FlatNetwork target)
        {
            EngineArray.ArrayCopy(weights, target.Weights);
            EngineArray.ArrayCopy(output, target.LayerOutput);
        }

        /// <summary>
        /// Copy the weights and output from the network.
        /// </summary>
        ///
        /// <param name="source">The network to copy from.</param>
        public void CopyFromNetwork(FlatNetwork source)
        {
            EngineArray.ArrayCopy(source.Weights, weights);
            EngineArray.ArrayCopy(source.LayerOutput, output);
        }
    }
}