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

namespace Encog.Util.Normalize.Output.Mapped
{
    /// <summary>
    /// An encoded output field.  This allows ranges of output values to be
    /// mapped to specific values.
    /// </summary>
    [Serializable]
    public class OutputFieldEncode : BasicOutputField
    {
        /// <summary>
        /// The ranges.
        /// </summary>
        private readonly IList<MappedRange> _ranges = new List<MappedRange>();

        /// <summary>
        /// The source field.
        /// </summary>
        private readonly IInputField _sourceField;

        /// <summary>
        /// The catch all value, if nothing matches, then use this value.
        /// </summary>
        private double _catchAll;


        /// <summary>
        /// Construct an encoded field.
        /// </summary>
        /// <param name="sourceField">The field that this is based on.</param>
        public OutputFieldEncode(IInputField sourceField)
        {
            _sourceField = sourceField;
        }

        /// <summary>
        /// The source field.
        /// </summary>
        public IInputField SourceField
        {
            get { return _sourceField; }
        }

        /// <summary>
        /// Return 1, no subfield supported.
        /// </summary>
        public override int SubfieldCount
        {
            get { return 1; }
        }

        /// <summary>
        /// The catch all value that is to be returned if none
        /// of the ranges match.
        /// </summary>
        public double CatchAll
        {
            get { return _catchAll; }
            set { _catchAll = value; }
        }

        /// <summary>
        /// Add a ranged mapped to a value.
        /// </summary>
        /// <param name="low">The low value for the range.</param>
        /// <param name="high">The high value for the range.</param>
        /// <param name="value">The value that the field should produce for this range.</param>
        public void AddRange(double low, double high, double value)
        {
            var range = new MappedRange(low, high, value);
            _ranges.Add(range);
        }

        /// <summary>
        /// Calculate the value for this field.
        /// </summary>
        /// <param name="subfield">Not used.</param>
        /// <returns>Return the value for the range the input falls within, or return
        /// the catchall if nothing matches.</returns>
        public override double Calculate(int subfield)
        {
            foreach (MappedRange range in _ranges)
            {
                if (range.InRange(_sourceField.CurrentValue))
                {
                    return range.Value;
                }
            }

            return _catchAll;
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }
}
