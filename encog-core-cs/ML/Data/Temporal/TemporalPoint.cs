//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
using System.Text;

namespace Encog.ML.Data.Temporal
{
    /// <summary>
    /// A point in time for a temporal data set.
    /// </summary>
    public class TemporalPoint : IComparable<TemporalPoint>
    {
        /// <summary>
        /// The data for this point.
        /// </summary>
        private double[] _data;

        /// <summary>
        /// The sequence number for this point.
        /// </summary>
        private int _sequence;

        /// <summary>
        /// Construct a temporal point of the specified size.
        /// </summary>
        /// <param name="size">The size to create the temporal point for.</param>
        public TemporalPoint(int size)
        {
            _data = new double[size];
        }

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        public double[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// The sequence number, used to sort.
        /// </summary>
        public int Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        /// <param name="x">The index.</param>
        /// <returns>The data at the specified index.</returns>
        public double this[int x]
        {
            get { return _data[x]; }
            set { _data[x] = value; }
        }

        #region IComparable<TemporalPoint> Members

        /// <summary>
        /// Compare two temporal points.
        /// </summary>
        /// <param name="that">The other temporal point to compare.</param>
        /// <returns>Returns 0 if they are equal, less than 0 if this point is less,
        /// greater than zero if this point is greater.</returns>
        public int CompareTo(TemporalPoint that)
        {
            if (Sequence == that.Sequence)
            {
                return 0;
            }
            if (Sequence < that.Sequence)
            {
                return -1;
            }
            return 1;
        }

        #endregion

        /**
         * Convert this point to string form.
         * @return This point as a string.
         */

        public override String ToString()
        {
            var builder = new StringBuilder("[TemporalPoint:");
            builder.Append("Seq:");
            builder.Append(_sequence);
            builder.Append(",Data:");
            for (int i = 0; i < _data.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }
                builder.Append(_data[i]);
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}
