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

namespace Encog.Util.Normalize.Output.Mapped
{
    /// <summary>
    /// Simple class that is used internally to hold a range mapping.
    /// </summary>
    [Serializable]
    public class MappedRange
    {
        /// <summary>
        /// The high value for the range.
        /// </summary>       
        private readonly double _high;

        /// <summary>
        /// The low value for the range.
        /// </summary>
        private readonly double _low;

        /// <summary>
        /// The value that should be returned for this range.
        /// </summary>
        private readonly double _value;

        /// <summary>
        /// Construct the range mapping.
        /// </summary>
        /// <param name="low">The low value for the range.</param>
        /// <param name="high">The high value for the range.</param>
        /// <param name="value">The value that this range represents.</param>
        public MappedRange(double low, double high, double value)
        {
            _low = low;
            _high = high;
            _value = value;
        }

        /// <summary>
        /// The high value for this range.
        /// </summary>
        public double High
        {
            get { return _high; }
        }

        /// <summary>
        /// The low value for this range.
        /// </summary>
        public double Low
        {
            get { return _low; }
        }

        /// <summary>
        /// The value that this range represents.
        /// </summary>
        public double Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Determine if the specified value is in the range.
        /// </summary>
        /// <param name="d">The value to check.</param>
        /// <returns>True if this value is within the range.</returns>
        public bool InRange(double d)
        {
            if ((d >= _low) && (d <= _high))
            {
                return true;
            }
            return false;
        }
    }
}
