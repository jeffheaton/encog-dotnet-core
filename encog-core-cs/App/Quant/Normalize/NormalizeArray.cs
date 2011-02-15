using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Normalize
{
    /// <summary>
    /// 
    /// </summary>
    public class NormalizeArray
    {
        /// <summary>
        /// Contains stats about the array normalized.
        /// </summary>
        private NormalizedFieldStats stats;

        /// <summary>
        /// Contains stats about the array normalized.
        /// </summary>
        public NormalizedFieldStats Stats { get { return this.stats; } }

        /// <summary>
        /// The high end of the range that the values are normalized into.  Typically 1.
        /// </summary>
        public double NormalizedHigh { get; set; }

        /// <summary>
        /// The low end of the range that the values are normalized into.  Typically 1.
        /// </summary>
        public double NormalizedLow { get; set; }

        /// <summary>
        /// Construct the object, default NormalizedHigh and NormalizedLow to 1 and -1.
        /// </summary>
        public NormalizeArray()
        {
            this.NormalizedHigh = 1;
            this.NormalizedLow = -1;
        }

        /// <summary>
        /// Normalize the array.  Return the new normalized array.
        /// </summary>
        /// <param name="inputArray">The input array.</param>
        /// <returns>The normalized array.</returns>
        public double[] Process(double[] inputArray)
        {
            this.stats = new NormalizedFieldStats();
            this.stats.NormalizedHigh = NormalizedHigh;
            this.stats.NormalizedLow = NormalizedLow;

            for (int i = 0; i < inputArray.Length; i++)
            {
                stats.Analyze(inputArray[i]);
            }

            double[] result = new double[inputArray.Length];

            for (int i = 0; i < inputArray.Length; i++)
            {
                result[i] = this.stats.Normalize(inputArray[i]);
            }

            return result;
        }
    }
}
