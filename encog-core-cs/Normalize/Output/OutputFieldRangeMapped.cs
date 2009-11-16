// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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
using Encog.Normalize.Input;

namespace Encog.Normalize.Output
{
    /// <summary>
    /// A ranged mapped output field.  This will scale the input so that it
    /// is between the high and low value.
    /// </summary>
    public class OutputFieldRangeMapped : BasicOutputField
    {
        /// <summary>
        /// The input field to scale.
        /// </summary>
        [EGReference]
        private IInputField field;

        /// <summary>
        /// The low value of the field.
        /// </summary>
        [EGAttribute]
        private double low;

        /// <summary>
        /// The high value of the field.
        /// </summary>
        [EGAttribute]
        private double high;

        /// <summary>
        /// Default constructor, used mainly for reflection.
        /// </summary>
        public OutputFieldRangeMapped()
        {

        }

        /// <summary>
        /// Construct a range mapped output field.
        /// </summary>
        /// <param name="field">The input field to base this on.</param>
        /// <param name="low">The low value.</param>
        /// <param name="high">The high value.</param>
        public OutputFieldRangeMapped(IInputField field, double low,
                 double high)
        {
            this.field = field;
            this.low = low;
            this.high = high;
        }

        /// <summary>
        /// Calculate this output field.
        /// </summary>
        /// <param name="subfield">Not used.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            return ((this.field.CurrentValue - this.field.Min) / (this.field
                    .Max - this.field.Min))
                    * (this.high - this.low) + this.low;
        }

        /// <summary>
        /// The field that this output is based on.
        /// </summary>
        public IInputField Field
        {
            get
            {
                return this.field;
            }
        }

        /// <summary>
        /// The high value of the range to map into.
        /// </summary>
        public double High
        {
            get
            {
                return this.high;
            }
        }

        /// <summary>
        /// The low value of the range to map into.
        /// </summary>
        public double Low
        {
            get
            {
                return this.low;
            }
        }

        /// <summary>
        /// This field only produces one value, so this will return 1.
        /// </summary>
        public override int SubfieldCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }
}
