namespace Encog.Util.Arrayutil
{
    /// <summary>
    /// Normalization is the process where data is adjusted to be inside a range.
    /// This range is typically -1 to 1. For more information about normalization,
    /// refer to the following page.
    /// http://www.heatonresearch.com/content/really-simple-introduction-
    /// normalization
    /// This class is used to normalize an array. Sometimes you would like to
    /// normalize an array, rather than an entire CSV file. If you would like to
    /// normalize an entire CSV file, you should make use of the class NormalizeCSV.
    /// </summary>
    ///
    public class NormalizeArray
    {
        /// <summary>
        /// The high end of the range that the values are normalized into. Typically
        /// 1.
        /// </summary>
        ///
        private double normalizedHigh;

        /// <summary>
        /// The low end of the range that the values are normalized into. Typically
        /// 1.
        /// </summary>
        ///
        private double normalizedLow;

        /// <summary>
        /// Contains stats about the array normalized.
        /// </summary>
        ///
        private NormalizedField stats;

        /// <summary>
        /// Construct the object, default NormalizedHigh and NormalizedLow to 1 and
        /// -1.
        /// </summary>
        ///
        public NormalizeArray()
        {
            normalizedHigh = 1;
            normalizedLow = -1;
        }

        /// <summary>
        /// Set the high value to normalize to.
        /// </summary>
        ///
        /// <value>The high value to normalize to.</value>
        public double NormalizedHigh
        {
            /// <returns>The high value to normalize to.</returns>
            get { return normalizedHigh; }
            /// <summary>
            /// Set the high value to normalize to.
            /// </summary>
            ///
            /// <param name="theNormalizedHigh">The high value to normalize to.</param>
            set { normalizedHigh = value; }
        }


        /// <summary>
        /// Set the low value to normalize to.
        /// </summary>
        ///
        /// <value>The low value to normalize to.</value>
        public double NormalizedLow
        {
            /// <returns>The low value to normalize to.</returns>
            get { return normalizedLow; }
            /// <summary>
            /// Set the low value to normalize to.
            /// </summary>
            ///
            /// <param name="theNormalizedLow">The low value to normalize to.</param>
            set { normalizedLow = value; }
        }


        /// <value>Contains stats about the array normalized.</value>
        public NormalizedField Stats
        {
            /// <returns>Contains stats about the array normalized.</returns>
            get { return stats; }
        }


        /// <summary>
        /// Normalize the array. Return the new normalized array.
        /// </summary>
        ///
        /// <param name="inputArray">The input array.</param>
        /// <returns>The normalized array.</returns>
        public double[] Process(double[] inputArray)
        {
            stats = new NormalizedField();
            stats.NormalizedHigh = normalizedHigh;
            stats.NormalizedLow = normalizedLow;


            foreach (double element  in  inputArray)
            {
                stats.Analyze(element);
            }

            var result = new double[inputArray.Length];

            for (int i = 0; i < inputArray.Length; i++)
            {
                result[i] = stats.Normalize(inputArray[i]);
            }

            return result;
        }
    }
}