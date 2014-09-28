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
    /// A direct output field, will simply pass the input value to the output.
    /// </summary>
    [Serializable]
    public class OutputFieldDirect : BasicOutputField
    {
        /// <summary>
        /// The source field.
        /// </summary>
        private readonly IInputField _sourceField;


        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public OutputFieldDirect()
        {
        }

        /// <summary>
        /// Construct a direct output field.
        /// </summary>
        /// <param name="sourceField">The source field to pass directly on.</param>
        public OutputFieldDirect(IInputField sourceField)
        {
            _sourceField = sourceField;
        }

        /// <summary>
        /// Always returns 1, as subfields are not used.
        /// </summary>
        public override int SubfieldCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Calculate the value for this field. This will simply be the
        /// value from the input field. 
        /// </summary>
        /// <param name="subfield">Not used, as this output field type does not
        /// support subfields.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            return _sourceField.CurrentValue;
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }
}
