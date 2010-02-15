// Encog(tm) Artificial Intelligence Framework v2.3
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
using Encog.Normalize.Input;

namespace Encog.Normalize.Output.Nominal
{
    /// <summary>
    /// An output field that uses the "on of" technique to represent input data. For
    /// example, if there were five nominal items, or classes, given then each one
    /// would be represented by a single output neuron that would be on or off.
    /// 
    /// Often the OutputEquilateral class is a better choice to represent nominal
    /// items.
    /// </summary>
    public class OutputOneOf : BasicOutputField
    {
        /// <summary>
        /// The nominal items to represent.
        /// </summary>
        private IList<NominalItem> items = new List<NominalItem>();

        /// <summary>
        /// What is the true value, often just "1".
        /// </summary>
        private double trueValue;

        /// <summary>
        /// What is the true value, often just "0" or "-1".
        /// </summary>
        private double falseValue;

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public OutputOneOf()
        {

        }

        /// <summary>
        /// Construct a one-of field and specify the true and false value.
        /// </summary>
        /// <param name="trueValue">The true value.</param>
        /// <param name="falseValue">The false value.</param>
        public OutputOneOf(double trueValue, double falseValue)
        {
            this.trueValue = trueValue;
            this.falseValue = falseValue;
        }

        /// <summary>
        /// Add a nominal value specifying a single value, the high and low values
        /// will be 0.5 below and 0.5 above.
        /// </summary>
        /// <param name="inputField">The input field to use.</param>
        /// <param name="value">The value to calculate the high and low values off of.</param>
        public void AddItem(IInputField inputField, double value)
        {
            AddItem(inputField, value - 0.5, value + 0.5);
        }

        /// <summary>
        /// Add a nominal item, specify the low and high values.
        /// </summary>
        /// <param name="inputField">The input field to base everything from.</param>
        /// <param name="low">The high value for this nominal item.</param>
        /// <param name="high">The low value for this nominal item.</param>
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
        /// <returns>The calculated value for this field.</returns>
        public override double Calculate(int subfield)
        {
            NominalItem item = this.items[subfield];
            return item.IsInRange() ? this.trueValue : this.falseValue;
        }

        /// <summary>
        /// The false value.
        /// </summary>
        public double FalseValue
        {
            get
            {
                return this.falseValue;
            }
        }

        /// <summary>
        /// The number of subfields, or nominal classes.
        /// </summary>
        /// <returns></returns>
        public override int SubfieldCount
        {
            get
            {
                return this.items.Count;
            }
        }

        /// <summary>
        /// The true value.
        /// </summary>
        /// <returns></returns>
        public double getTrueValue()
        {
            return this.trueValue;
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }

    }
}
