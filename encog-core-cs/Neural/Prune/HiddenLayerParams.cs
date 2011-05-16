namespace Encog.Neural.Prune
{
    /// <summary>
    /// Specifies the minimum and maximum neuron counts for a layer.
    /// </summary>
    ///
    public class HiddenLayerParams
    {
        /// <summary>
        /// The maximum number of neurons on this layer.
        /// </summary>
        ///
        private readonly int max;

        /// <summary>
        /// The minimum number of neurons on this layer.
        /// </summary>
        ///
        private readonly int min;

        /// <summary>
        /// Construct a hidden layer param object with the specified min and max
        /// values.
        /// </summary>
        ///
        /// <param name="min_0">The minimum number of neurons.</param>
        /// <param name="max_1">The maximum number of neurons.</param>
        public HiddenLayerParams(int min_0, int max_1)
        {
            min = min_0;
            max = max_1;
        }


        /// <value>The maximum number of neurons.</value>
        public int Max
        {
            /// <returns>The maximum number of neurons.</returns>
            get { return max; }
        }


        /// <value>The minimum number of neurons.</value>
        public int Min
        {
            /// <returns>The minimum number of neurons.</returns>
            get { return min; }
        }
    }
}