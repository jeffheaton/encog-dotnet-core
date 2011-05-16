using Encog.ML.Data;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// Used to map one neural data object to another. Useful for a BAM network.
    /// </summary>
    ///
    public class NeuralDataMapping
    {
        /// <summary>
        /// Construct the neural data mapping class, with null values.
        /// </summary>
        ///
        public NeuralDataMapping()
        {
            From = null;
            To = null;
        }

        /// <summary>
        /// Construct the neural data mapping class with the specified values.
        /// </summary>
        ///
        /// <param name="from_0">The source data.</param>
        /// <param name="to_1">The target data.</param>
        public NeuralDataMapping(MLData from_0, MLData to_1)
        {
            From = from_0;
            To = to_1;
        }

        /// <summary>
        /// Set the from data.
        /// </summary>
        ///
        /// <value>The from data.</value>
        public MLData From { /// <returns>The "from" data.</returns>
            get; /// <summary>
            /// Set the from data.
            /// </summary>
            ///
            /// <param name="from_0">The from data.</param>
            set; }


        /// <summary>
        /// Set the target data.
        /// </summary>
        ///
        /// <value>The target data.</value>
        public MLData To { /// <returns>The "to" data.</returns>
            get; /// <summary>
            /// Set the target data.
            /// </summary>
            ///
            /// <param name="to_0">The target data.</param>
            set; }

        /// <summary>
        /// Copy from one object to the other.
        /// </summary>
        ///
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        public static void Copy(NeuralDataMapping source,
                                NeuralDataMapping target)
        {
            for (int i = 0; i < source.From.Count; i++)
            {
                target.From[i] = source.From[i];
            }

            for (int i_0 = 0; i_0 < source.To.Count; i_0++)
            {
                target.To[i_0] = source.To[i_0];
            }
        }
    }
}