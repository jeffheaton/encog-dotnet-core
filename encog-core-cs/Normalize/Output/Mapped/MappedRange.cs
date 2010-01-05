// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Attributes;

namespace Encog.Normalize.Output.Mapped
{
    /// <summary>
    /// Simple class that is used internally to hold a range mapping.
    /// </summary>
    public class MappedRange
    {
        /// <summary>
        /// The low value for the range.
        /// </summary>
        [EGAttribute]
        private double low;

        /// <summary>
        /// The high value for the range.
        /// </summary>
        [EGAttribute]
        private double high;

        /// <summary>
        /// The value that should be returned for this range.
        /// </summary>
        [EGAttribute]
        private double value;

        /// <summary>
        /// Construct the range mapping.
        /// </summary>
        /// <param name="low">The low value for the range.</param>
        /// <param name="high">The high value for the range.</param>
        /// <param name="value">The value that this range represents.</param>
        public MappedRange(double low, double high, double value)
        {
            this.low = low;
            this.high = high;
            this.value = value;
        }

        /// <summary>
        /// The high value for this range.
        /// </summary>
        public double High
        {
            get
            {
                return this.high;
            }
        }

        /// <summary>
        /// The low value for this range.
        /// </summary>
        public double Low
        {
            get
            {
                return this.low;
            }
        }

        /// <summary>
        /// The value that this range represents.
        /// </summary>
        public double Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Determine if the specified value is in the range.
        /// </summary>
        /// <param name="d">The value to check.</param>
        /// <returns>True if this value is within the range.</returns>
        public bool InRange(double d)
        {
            if ((d >= this.low) && (d <= this.high))
            {
                return true;
            }
            return false;
        }
    }
}
