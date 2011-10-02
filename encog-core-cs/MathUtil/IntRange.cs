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
using Encog.Fuzzy.Core;

namespace Encog.MathUtil
{
    /// <summary>
    /// A range of integers.
    /// </summary>
    [Serializable]
    public class IntRange
    {
        /// <summary>
        /// Construct an integer range.
        /// </summary>
        /// <param name="high">The high  end of the range.</param>
        /// <param name="low">The low end of the range.</param>
        public IntRange(int high, int low)
        {
            High = high;
            Low = low;
        }

        /// <summary>
        /// The low end of the range.
        /// </summary>
        public int High { get; set; }

        /// <summary>
        /// The high end of the range.
        /// </summary>
        public int Low { get; set; }

        /// <summary>
        /// Length of the range (difference between maximum and minimum values).
        /// </summary>
        public int Length
        {
            get { return High - Low; }
        }

        /// <summary>
        /// Check if the specified value is inside of the range.
        /// </summary>
        /// 
        /// <param name="x">Value to check.</param>
        /// 
        /// <returns><b>True</b> if the specified value is inside of the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsInside(int x)
        {
            return ((x >= Low) && (x <= High));
        }

        /// <summary>
        /// Check if the specified range is inside of the range.
        /// </summary>
        /// 
        /// <param name="range">Range to check.</param>
        /// 
        /// <returns><b>True</b> if the specified range is inside of the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsInside(IntRange range)
        {
            return ((IsInside(range.Low)) && (IsInside(range.High)));
        }

        /// <summary>
        /// Check if the specified range overlaps with the range.
        /// </summary>
        /// 
        /// <param name="range">Range to check for overlapping.</param>
        /// 
        /// <returns><b>True</b> if the specified range overlaps with the range or
        /// <b>false</b> otherwise.</returns>
        /// 
        public bool IsOverlapping(IntRange range)
        {
            return ((IsInside(range.Low)) || (IsInside(range.High)) ||
                     (range.IsInside(Low)) || (range.IsInside(High)));
        }

        /// <summary>
        /// Implicit conversion to <see cref="Range"/>.
        /// </summary>
        /// 
        /// <param name="range">Integer range to convert to single precision range.</param>
        /// 
        /// <returns>Returns new single precision range which min/max values are implicitly converted
        /// to floats from min/max values of the specified integer range.</returns>
        /// 
        public static implicit operator Range(IntRange range)
        {
            return new Range(range.Low, range.High);
        }

        /// <summary>
        /// Equality operator - checks if two ranges have equal min/max values.
        /// </summary>
        /// 
        /// <param name="range1">First range to check.</param>
        /// <param name="range2">Second range to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if min/max values of specified
        /// ranges are equal.</returns>
        ///
        public static bool operator ==(IntRange range1, IntRange range2)
        {
            return ((range1.Low == range2.Low) && (range1.High == range2.High));
        }

        /// <summary>
        /// Inequality operator - checks if two ranges have different min/max values.
        /// </summary>
        /// 
        /// <param name="range1">First range to check.</param>
        /// <param name="range2">Second range to check.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if min/max values of specified
        /// ranges are not equal.</returns>
        ///
        public static bool operator !=(IntRange range1, IntRange range2)
        {
            return ((range1.Low != range2.Low) || (range1.High != range2.High));

        }

        /// <summary>
        /// Check if this instance of <see cref="Range"/> equal to the specified one.
        /// </summary>
        /// 
        /// <param name="obj">Another range to check equalty to.</param>
        /// 
        /// <returns>Return <see langword="true"/> if objects are equal.</returns>
        /// 
        public override bool Equals(object obj)
        {
            return (obj is IntRange) ? (this == (IntRange)obj) : false;
        }

        /// <summary>
        /// Get hash code for this instance.
        /// </summary>
        /// 
        /// <returns>Returns the hash code for this instance.</returns>
        /// 
        public override int GetHashCode()
        {
            return Low.GetHashCode() + High.GetHashCode();
        }

        /// <summary>
        /// Get string representation of the class.
        /// </summary>
        /// 
        /// <returns>Returns string, which contains min/max values of the range in readable form.</returns>
        ///
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}", Low, High);
        }
    }
}
