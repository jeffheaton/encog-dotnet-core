// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Attributes;

namespace Encog.Normalize.Segregate
{
    /// <summary>
    /// Specifies a range that might be included or excluded.
    /// </summary>
    public class SegregationRange
    {
        /// <summary>
        /// The low end of this range.
        /// </summary>
        [EGAttribute]
        private double low;

        /// <summary>
        /// The high end of this range.
        /// </summary>
        [EGAttribute]
        private double high;

        /// <summary>
        /// Should this range be included.
        /// </summary>
        [EGAttribute]
        private bool include;

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

            this.low = low;
            this.high = high;
            this.include = include;
        }

        /// <summary>
        /// The high end of the range.
        /// </summary>
        public double High
        {
            get
            {
                return this.high;
            }
        }

        /// <summary>
        /// The low end of the range.
        /// </summary>
        public double Low
        {
            get
            {
                return this.low;
            }
        }

        /// <summary>
        /// Is this value within the range. 
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is within the range.</returns>
        public bool InRange(double value)
        {
            return ((value >= this.low) && (value <= this.high));
        }

        /// <summary>
        /// True if this range should be included.
        /// </summary>
        public bool IsIncluded
        {
            get
            {
                return this.include;
            }
        }
    }
}
