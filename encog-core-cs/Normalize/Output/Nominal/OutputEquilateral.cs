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
using Encog.Util.MathUtil;
using Encog.Normalize.Input;
using Encog.Persist.Attributes;

namespace Encog.Normalize.Output.Nominal
{
    /// <summary>
    /// Allows nominal items to be encoded using the equilateral method. This maps
    /// the nominal items into an array of input or output values minus 1. This can
    /// sometimes provide a more accurate representation than the "one of" method.
    /// Based on: Guiver and Klimasauskas (1991).
    /// </summary>
    public class OutputEquilateral : BasicOutputField
    {
        /// <summary>
        /// The nominal items.
        /// </summary>
        private IList<NominalItem> items = new List<NominalItem>();

        /// <summary>
        /// The current equilateral matrix.
        /// </summary>
        [EGIgnore]
        private Equilateral equilateral;

        /// <summary>
        /// The current value, which nominal item is selected.
        /// </summary>
        private int currentValue;

        /// <summary>
        /// The high value to map into.
        /// </summary>
        private double high;

        /// <summary>
        /// The low value to map into.
        /// </summary>
        private double low;

        /// <summary>
        /// Prodvide a default constructor for reflection.
        /// </summary>
        public OutputEquilateral()
        {

        }

        /// <summary>
        /// Create an equilateral output field with the specified high and low output
        /// values. These will often be 0 to 1 or -1 to 1.
        /// </summary>
        /// <param name="high">The high output value.</param>
        /// <param name="low">The low output value.</param>
        public OutputEquilateral(double high, double low)
        {
            this.high = high;
            this.low = low;
        }

        /// <summary>
        /// Add a nominal value based on a single value.  This creates a 0.1 range
        /// around this value.
        /// </summary>
        /// <param name="inputField">The input field this is based on.</param>
        /// <param name="value">The value.</param>
        public void AddItem(IInputField inputField, double value)
        {
            AddItem(inputField, value + 0.1, value - 0.1);
        }

        /// <summary>
        /// Add a nominal item based on a range.
        /// </summary>
        /// <param name="inputField">The input field to use.</param>
        /// <param name="low">The low value of the range.</param>
        /// <param name="high">The high value of the range.</param>
        public void AddItem(IInputField inputField, double low,
                 double high)
        {
            NominalItem item = new NominalItem(inputField, low, high);
            this.items.Add(item);
        }

        /// <summary>
        /// Calculate the value for the specified subfield.
        /// </summary>
        /// <param name="subfield">The subfield to calculate for.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            return this.equilateral.Encode(this.currentValue)[subfield];
        }

        /// <summary>
        /// The equalateral table being used.
        /// </summary>
        public Equilateral Equilateral
        {
            get
            {
                return this.equilateral;
            }
        }

        /// <summary>
        /// The high value of the range.
        /// </summary>
        public double getHigh()
        {
            return this.high;
        }

        /// <summary>
        /// The low value of the range.
        /// </summary>
        public double getLow()
        {
            return this.low;
        }

        /// <summary>
        /// This is the total number of nominal items minus 1.
        /// </summary>
        public override int SubfieldCount
        {
            get
            {
                return this.items.Count - 1;
            }
        }

        /// <summary>
        /// Determine which item's index is the value.
        /// </summary>
        public override void RowInit()
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                NominalItem item = this.items[i];
                if (item.IsInRange())
                {
                    this.currentValue = i;
                    break;
                }
            }

            if (this.equilateral == null)
            {
                this.equilateral = new Equilateral(this.items.Count, this.high,
                        this.low);
            }
        }
    }
}
