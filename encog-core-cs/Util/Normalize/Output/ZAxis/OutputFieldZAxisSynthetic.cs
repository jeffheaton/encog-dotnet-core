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

namespace Encog.Util.Normalize.Output.ZAxis
{
    /// <summary>
    /// This field represents the synthetic value used in Z-Axis normalization.
    /// For more information see the OutputFieldZAxis class.
    /// </summary>
    [Serializable]
    public class OutputFieldZAxisSynthetic : OutputFieldGrouped
    {
        /// <summary>
        /// Construct a synthetic output field for Z-Axis.
        /// </summary>
        /// <param name="group">The Z-Axis group that this belongs to.</param>
        public OutputFieldZAxisSynthetic(IOutputFieldGroup group)
            : base(group, null)
        {
            if (!(group is ZAxisGroup))
            {
                throw new NormalizationError(
                    "Must use ZAxisGroup with OutputFieldZAxisSynthetic.");
            }
        }

        /// <summary>
        /// The subfield count, which is one, as this field type does not
        /// have subfields.
        /// </summary>
        public override int SubfieldCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Calculate the synthetic value for this Z-Axis normalization.
        /// </summary>
        /// <param name="subfield">Not used.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            double l = ((ZAxisGroup) Group).Length;
            double f = ((ZAxisGroup) Group).Multiplier;
            double n = Group.GroupedFields.Count;
            double result = f*Math.Sqrt(n - (l*l));
            return double.IsInfinity(result) || double.IsNaN(result) ? 0 : result;
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }
}
