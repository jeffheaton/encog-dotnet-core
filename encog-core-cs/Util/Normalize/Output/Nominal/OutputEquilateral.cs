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
using Encog.MathUtil;
using Encog.Util.Normalize.Input;

namespace Encog.Util.Normalize.Output.Nominal
{
    /// <summary>
    /// Allows nominal items to be encoded using the equilateral method. This maps
    /// the nominal items into an array of input or output values minus 1. This can
    /// sometimes provide a more accurate representation than the "one of" method.
    /// Based on: Guiver and Klimasauskas (1991).
    /// </summary>
    [Serializable]
    public class OutputEquilateral : BasicOutputField
    {
        /// <summary>
        /// The high value to map into.
        /// </summary>
        private readonly double _high;

        /// <summary>
        /// The nominal items.
        /// </summary>
        private readonly IList<NominalItem> _items = new List<NominalItem>();

        /// <summary>
        /// The low value to map into.
        /// </summary>
        private readonly double _low;

        /// <summary>
        /// The current value, which nominal item is selected.
        /// </summary>
        private int _currentValue;

        /// <summary>
        /// The current equilateral matrix.
        /// </summary>
        private Equilateral _equilateral;

        /// <summary>
        /// Prodvide a default constructor for reflection.
        /// Use -1 for low and +1 for high.
        /// </summary>
        public OutputEquilateral()
            : this(1, -1)
        {
        }

        /// <summary>
        /// Create an equilateral output field with the specified high and low output
        /// values. These will often be 0 to 1 or -1 to 1.
        /// </summary>
        /// <param name="high">The high output value.</param>
        /// <param name="low">The low output value.</param>
        public OutputEquilateral(double low, double high)
        {
            _high = high;
            _low = low;
        }

        /// <summary>
        /// The equalateral table being used.
        /// </summary>
        public Equilateral Equilateral
        {
            get { return _equilateral; }
        }

        /// <summary>
        /// This is the total number of nominal items minus 1.
        /// </summary>
        public override int SubfieldCount
        {
            get { return _items.Count - 1; }
        }

        /// <summary>
        /// Add a nominal value based on a single value.  This creates a 0.1 range
        /// around this value.
        /// </summary>
        /// <param name="inputField">The input field this is based on.</param>
        /// <param name="value">The value.</param>
        public void AddItem(IInputField inputField, double value)
        {
            AddItem(inputField, value - 0.1, value + 0.1);
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
            var item = new NominalItem(inputField, low, high);
            _items.Add(item);
        }

        /// <summary>
        /// Calculate the value for the specified subfield.
        /// </summary>
        /// <param name="subfield">The subfield to calculate for.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            return _equilateral.Encode(_currentValue)[subfield];
        }

        /// <summary>
        /// The high value of the range.
        /// </summary>
        public double High
        {
            get { return _high; }
        }

        /// <summary>
        /// The low value of the range.
        /// </summary>
        public double Low
        {
            get { return _low; }
        }

        /// <summary>
        /// Determine which item's index is the value.
        /// </summary>
        public override void RowInit()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                NominalItem item = _items[i];
                if (item.IsInRange())
                {
                    _currentValue = i;
                    break;
                }
            }

            if (_equilateral == null)
            {
                _equilateral = new Equilateral(_items.Count, _high,
                                              _low);
            }
        }
    }
}
