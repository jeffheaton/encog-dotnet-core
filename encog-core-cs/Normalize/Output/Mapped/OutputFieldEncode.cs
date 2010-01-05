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
using Encog.Normalize.Input;

namespace Encog.Normalize.Output.Mapped
{
    /// <summary>
    /// An encoded output field.  This allows ranges of output values to be
    /// mapped to specific values.
    /// </summary>
    public class OutputFieldEncode : BasicOutputField
    {
        /// <summary>
        /// The source field.
        /// </summary>
        private IInputField sourceField;

        /// <summary>
        /// The catch all value, if nothing matches, then use this value.
        /// </summary>
        private double catchAll;

        /// <summary>
        /// The ranges.
        /// </summary>
        private IList<MappedRange> ranges = new List<MappedRange>();


        /// <summary>
        /// Construct an encoded field.
        /// </summary>
        /// <param name="sourceField">The field that this is based on.</param>
        public OutputFieldEncode(IInputField sourceField)
        {
            this.sourceField = sourceField;
        }

        /// <summary>
        /// Add a ranged mapped to a value.
        /// </summary>
        /// <param name="low">The low value for the range.</param>
        /// <param name="high">The high value for the range.</param>
        /// <param name="value">The value that the field should produce for this range.</param>
        public void addRange(double low, double high, double value)
        {
            MappedRange range = new MappedRange(low, high, value);
            this.ranges.Add(range);
        }

        /// <summary>
        /// Calculate the value for this field.
        /// </summary>
        /// <param name="subfield">Not used.</param>
        /// <returns>Return the value for the range the input falls within, or return
        /// the catchall if nothing matches.</returns>
        public override double Calculate(int subfield)
        {
            foreach (MappedRange range in this.ranges)
            {
                if (range.InRange(this.sourceField.CurrentValue))
                {
                    return range.Value;
                }
            }

            return this.catchAll;
        }

        /// <summary>
        /// The source field.
        /// </summary>
        public IInputField SourceField
        {
            get
            {
                return this.sourceField;
            }
        }

        /// <summary>
        /// Return 1, no subfield supported.
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

        /// <summary>
        /// The catch all value that is to be returned if none
        /// of the ranges match.
        /// </summary>
        public double CatchAll
        {
            get
            {
                return this.catchAll;
            }
            set
            {
                this.catchAll = value;
            }
        }
    }
}
