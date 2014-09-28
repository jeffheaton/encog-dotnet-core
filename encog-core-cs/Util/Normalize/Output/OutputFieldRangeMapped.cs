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

namespace Encog.Util.Normalize.Output
{
    /// <summary>
    /// A ranged mapped output field.  This will scale the input so that it
    /// is between the high and low value.
    /// </summary>
    [Serializable]
    public class OutputFieldRangeMapped : BasicOutputField, IRequireTwoPass
    {
        /// <summary>
        /// The input field to scale.
        /// </summary>
        private readonly IInputField _field;

        /// <summary>
        /// The high value of the field.
        /// </summary>
        private readonly double _high;

        /// <summary>
        /// The low value of the field.
        /// </summary>
        private readonly double _low;

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
            _field = field;
            _low = low;
            _high = high;
        }

        /// <summary>
        /// Construct the output field with -1 for low and +1 for high.
        /// </summary>
        /// <param name="f">The input field.</param>
        public OutputFieldRangeMapped(IInputField f): this(f,-1,1)
        {
        }

        /// <summary>
        /// The field that this output is based on.
        /// </summary>
        public IInputField Field
        {
            get { return _field; }
        }

        /// <summary>
        /// The high value of the range to map into.
        /// </summary>
        public double High
        {
            get { return _high; }
        }

        /// <summary>
        /// The low value of the range to map into.
        /// </summary>
        public double Low
        {
            get { return _low; }
        }

        /// <summary>
        /// This field only produces one value, so this will return 1.
        /// </summary>
        public override int SubfieldCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Calculate this output field.
        /// </summary>
        /// <param name="subfield">Not used.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            return ((_field.CurrentValue - _field.Min)/(_field
                                                          .Max - _field.Min))
                   *(_high - _low) + _low;
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }

        /// <summary>
        /// Convert a number back after its been normalized.
        /// </summary>
        /// <param name="data">The number to convert back.</param>
        /// <returns>The result.</returns>
        public double ConvertBack(double data)
        {
            double result = ((_field.Min - _field.Max)*data - _high
                                                            *_field.Min + _field.Max*_low)
                            /(_low - _high);
            return result;
        }
    }
}
