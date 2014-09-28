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

namespace Encog.Util.Normalize.Segregate
{
    /// <summary>
    /// Specifies a range that might be included or excluded.
    /// </summary>
    [Serializable]
    public class SegregationRange
    {
        /// <summary>
        /// The high end of this range.
        /// </summary>
        private readonly double _high;

        /// <summary>
        /// Should this range be included.
        /// </summary>
        private readonly bool _include;

        /// <summary>
        /// The low end of this range.
        /// </summary>
        private readonly double _low;

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public SegregationRange()
        {
        }

        /// <summary>
        /// Construct a segregation range.
        /// </summary>
        /// <param name="low">The low end of the range.</param>
        /// <param name="high">The high end of the range.</param>
        /// <param name="include">Specifies if the range should be included.</param>
        public SegregationRange(double low, double high,
                                bool include)
        {
            _low = low;
            _high = high;
            _include = include;
        }

        /// <summary>
        /// The high end of the range.
        /// </summary>
        public double High
        {
            get { return _high; }
        }

        /// <summary>
        /// The low end of the range.
        /// </summary>
        public double Low
        {
            get { return _low; }
        }

        /// <summary>
        /// True if this range should be included.
        /// </summary>
        public bool IsIncluded
        {
            get { return _include; }
        }

        /// <summary>
        /// Is this value within the range. 
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is within the range.</returns>
        public bool InRange(double value)
        {
            return ((value >= _low) && (value <= _high));
        }
    }
}
