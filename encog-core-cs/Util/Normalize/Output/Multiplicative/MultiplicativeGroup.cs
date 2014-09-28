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
using System.Linq;

namespace Encog.Util.Normalize.Output.Multiplicative
{
    /// <summary>
    /// Used to group multiplicative fields together.
    /// </summary>
    [Serializable]
    public class MultiplicativeGroup : BasicOutputFieldGroup
    {
        /// <summary>
        /// The "length" of this field.
        /// </summary>
        private double _length;

        /// <summary>
        /// The length of this field.  This is the sum of the squares of
        /// all of the groupped fields.  The square root of this sum is the 
        /// length. 
        /// </summary>
        public double Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Called to init this group for a new field.  This recalculates the
        /// "length".
        /// </summary>
        public override void RowInit()
        {
            double value = GroupedFields.Sum(field => (field.SourceField.CurrentValue*field.SourceField.CurrentValue));
            _length = Math.Sqrt(value);
        }
    }
}
