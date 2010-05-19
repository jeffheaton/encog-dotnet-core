using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util;

namespace Encog.MathUtil
{
    /// <summary>
    /// A numeric range has a high, low, mean, root-mean-square, standard deviation,
    /// and the count of how many samples it contains.
    /// </summary>
    public class NumericRange
    {
        /// <summary>
        /// The high number in the range.
        /// </summary>
        private double high;

        /// <summary>
        /// The low number in the range.
        /// </summary>
        private double low;

        /// <summary>
        /// The mean value.
        /// </summary>
        private double mean;

        /// <summary>
        /// The root mean square of the range.
        /// </summary>
        private double rms;

        /// <summary>
        /// The standard deviation of the range.
        /// </summary>
        private double standardDeviation;

        /// <summary>
        /// The number of values in this range.
        /// </summary>
        private int samples;

        /// <summary>
        /// Create a numeric range from a list of values. 
        /// </summary>
        /// <param name="values">The values to calculate for.</param>
        public NumericRange(IList<Double> values)
        {

            double assignedHigh = 0;
            double assignedLow = 0;
            double total = 0;
            double rmsTotal = 0;

            // get the mean and other 1-pass values.

            foreach (double d in values)
            {
                assignedHigh = Math.Max(assignedHigh, d);
                assignedLow = Math.Min(assignedLow, d);
                total += d;
                rmsTotal += d * d;
            }

            this.samples = values.Count;
            this.high = assignedHigh;
            this.low = assignedLow;
            this.mean = total / this.samples;
            this.rms = Math.Sqrt(rmsTotal / this.samples);

            // now get the standard deviation
            double devTotal = 0;

            foreach (double d in values)
            {
                devTotal += Math.Pow(d - this.mean, 2);
            }
            this.standardDeviation = Math.Sqrt(devTotal / this.samples);
        }

        /// <summary>
        /// The high number in the range.
        /// </summary>
        public double High
        {
            get
            {
                return high;
            }
        }

        /// <summary>
        /// The low number in the range.
        /// </summary>
        public double Low
        {
            get
            {
                return low;
            }
        }

        /// <summary>
        /// The mean in the range.
        /// </summary>
        public double getMean
        {
            get
            {
                return mean;
            }
        }

        /// <summary>
        /// The root mean square of the range.
        /// </summary>
        public double RMS
        {
            get
            {
                return rms;
            }
        }

        /// <summary>
        /// The standard deviation of the range.
        /// </summary>
        public double StandardDeviation
        {
            get
            {
                return standardDeviation;
            }
        }

        /// <summary>
        /// The number of samples in the range.
        /// </summary>
        public int Samples
        {
            get
            {
                return samples;
            }
        }

        /// <summary>
        /// The range as a string.
        /// </summary>
        /// <returns>The range as a string.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("Range: ");
            result.Append(Format.FormatDouble(this.low, 5));
            result.Append(" to ");
            result.Append(Format.FormatDouble(this.high, 5));
            result.Append(",samples: ");
            result.Append(Format.FormatInteger(this.samples));
            result.Append(",mean: ");
            result.Append(Format.FormatDouble(this.mean, 5));
            result.Append(",rms: ");
            result.Append(Format.FormatDouble(this.rms, 5));
            result.Append(",s.deviation: ");
            result.Append(Format.FormatDouble(this.standardDeviation, 5));

            return result.ToString();
        }
    }
}
