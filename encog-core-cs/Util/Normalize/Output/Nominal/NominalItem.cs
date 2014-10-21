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
using Encog.Util.Normalize.Input;

namespace Encog.Util.Normalize.Output.Nominal
{
    /// <summary>
    /// A nominal item.
    /// </summary>
    [Serializable]
    public class NominalItem
    {
        /// <summary>
        /// The high value for the range.
        /// </summary>
        private readonly double _high;

        /// <summary>
        /// The input field used to verify against the range.
        /// </summary>
        private readonly IInputField _inputField;

        /// <summary>
        /// The low value for the range.
        /// </summary>
        private readonly double _low;

        /// <summary>
        /// Construct a empty range item.  Used mainly for reflection.
        /// </summary>
        public NominalItem()
        {
        }

        /// <summary>
        /// Create a nominal item.
        /// </summary>
        /// <param name="inputField">The field that this item is based on.</param>
        /// <param name="high">The high value.</param>
        /// <param name="low">The low value.</param>
        public NominalItem(IInputField inputField, double low,
                           double high)
        {
            _high = high;
            _low = low;
            _inputField = inputField;
        }

        /// <summary>
        /// The high value.
        /// </summary>
        public double High
        {
            get { return _high; }
        }

        /// <summary>
        /// The input field value.
        /// </summary>
        public IInputField InputField
        {
            get { return _inputField; }
        }

        /// <summary>
        /// The low value.
        /// </summary>
        public double Low
        {
            get { return _low; }
        }

        /// <summary>
        /// Begin a row.
        /// </summary>
        public void BeginRow()
        {
        }

        /// <summary>
        /// Determine if the specified value is in range.
        /// </summary>
        /// <returns>True if this item is within range.</returns>
        public bool IsInRange()
        {
            double currentValue = _inputField.CurrentValue;
            return ((currentValue >= _low) && (currentValue <= _high));
        }
    }
}
