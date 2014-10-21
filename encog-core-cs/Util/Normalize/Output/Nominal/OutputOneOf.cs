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
using System.Collections.Generic;
using Encog.Util.Normalize.Input;

namespace Encog.Util.Normalize.Output.Nominal
{
    /// <summary>
    /// An output field that uses the "on of" technique to represent input data. For
    /// example, if there were five nominal items, or classes, given then each one
    /// would be represented by a single output neuron that would be on or off.
    /// 
    /// Often the OutputEquilateral class is a better choice to represent nominal
    /// items.
    /// </summary>
    [Serializable]
    public class OutputOneOf : BasicOutputField
    {
        /// <summary>
        /// What is the true value, often just "0" or "-1".
        /// </summary>
        private readonly double _falseValue;

        /// <summary>
        /// The nominal items to represent.
        /// </summary>
        private readonly IList<NominalItem> _items = new List<NominalItem>();

        /// <summary>
        /// What is the true value, often just "1".
        /// </summary>
        private readonly double _trueValue;

        /// <summary>
        /// Default constructor for reflection.  Use 1 for true, -1 for false.
        /// </summary>
        public OutputOneOf(): this(1,-1)
        {
        }

        /// <summary>
        /// Construct a one-of field and specify the true and false value.
        /// </summary>
        /// <param name="trueValue">The true value.</param>
        /// <param name="falseValue">The false value.</param>
        public OutputOneOf(double trueValue, double falseValue)
        {
            _trueValue = trueValue;
            _falseValue = falseValue;
        }

        /// <summary>
        /// The false value.
        /// </summary>
        public double FalseValue
        {
            get { return _falseValue; }
        }

        /// <summary>
        /// The number of subfields, or nominal classes.
        /// </summary>
        /// <returns></returns>
        public override int SubfieldCount
        {
            get { return _items.Count; }
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
            var item = new NominalItem(inputField, low, high);
            _items.Add(item);
        }

        /// <summary>
        /// Calculate the value for the specified subfield.
        /// </summary>
        /// <param name="subfield">The subfield to calculate for.</param>
        /// <returns>The calculated value for this field.</returns>
        public override double Calculate(int subfield)
        {
            NominalItem item = _items[subfield];
            return item.IsInRange() ? _trueValue : _falseValue;
        }

        /// <summary>
        /// The true value.
        /// </summary>
        /// <returns></returns>
        public double TrueValue
        {
            get { return _trueValue; }
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }
}
