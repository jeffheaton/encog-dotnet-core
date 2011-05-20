//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
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
        private readonly double high;

        /// <summary>
        /// The low number in the range.
        /// </summary>
        private readonly double low;

        /// <summary>
        /// The mean value.
        /// </summary>
        private readonly double mean;

        /// <summary>
        /// The root mean square of the range.
        /// </summary>
        private readonly double rms;

        /// <summary>
        /// The number of values in this range.
        /// </summary>
        private readonly int samples;

        /// <summary>
        /// The standard deviation of the range.
        /// </summary>
        private readonly double standardDeviation;

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
                rmsTotal += d*d;
            }

            samples = values.Count;
            high = assignedHigh;
            low = assignedLow;
            mean = total/samples;
            rms = Math.Sqrt(rmsTotal/samples);

            // now get the standard deviation
            double devTotal = 0;

            foreach (double d in values)
            {
                devTotal += Math.Pow(d - mean, 2);
            }
            standardDeviation = Math.Sqrt(devTotal/samples);
        }

        /// <summary>
        /// The high number in the range.
        /// </summary>
        public double High
        {
            get { return high; }
        }

        /// <summary>
        /// The low number in the range.
        /// </summary>
        public double Low
        {
            get { return low; }
        }

        /// <summary>
        /// The mean in the range.
        /// </summary>
        public double getMean
        {
            get { return mean; }
        }

        /// <summary>
        /// The root mean square of the range.
        /// </summary>
        public double RMS
        {
            get { return rms; }
        }

        /// <summary>
        /// The standard deviation of the range.
        /// </summary>
        public double StandardDeviation
        {
            get { return standardDeviation; }
        }

        /// <summary>
        /// The number of samples in the range.
        /// </summary>
        public int Samples
        {
            get { return samples; }
        }

        /// <summary>
        /// The range as a string.
        /// </summary>
        /// <returns>The range as a string.</returns>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("Range: ");
            result.Append(Format.FormatDouble(low, 5));
            result.Append(" to ");
            result.Append(Format.FormatDouble(high, 5));
            result.Append(",samples: ");
            result.Append(Format.FormatInteger(samples));
            result.Append(",mean: ");
            result.Append(Format.FormatDouble(mean, 5));
            result.Append(",rms: ");
            result.Append(Format.FormatDouble(rms, 5));
            result.Append(",s.deviation: ");
            result.Append(Format.FormatDouble(standardDeviation, 5));

            return result.ToString();
        }
    }
}
