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

namespace Encog.Util.Normalize.Output.ZAxis
{
    /// <summary>
    /// Both the multiplicative and z-axis normalization types allow a group of 
    /// outputs to be adjusted so that the "vector length" is 1.  Both go about it
    /// in different ways.  Certain types of neural networks require a vector length 
    /// of 1.
    /// 
    /// Z-Axis normalization is usually a better choice than multiplicative.    
    /// However, multiplicative can perform better than Z-Axis when all of the 
    /// values are near zero most of the time.  This can cause the "synthetic value"
    /// that z-axis uses to dominate and skew the answer.
    /// 
    ///  Z-Axis gets its name from 3D computer graphics, where there is a Z-Axis
    ///  extending from the plane created by the X and Y axes.  It has nothing to 
    ///  do with z-scores or the z-transform of signal theory.
    ///  
    ///  To implement Z-Axis normalization a scaling factor must be created to multiply
    ///  each of the inputs against.  Additionally, a synthetic field must be added.
    ///  It is very important that this synthetic field be added to any z-axis
    ///  group that you might use.  The synthetic field is represented by the
    ///  OutputFieldZAxisSynthetic class.
    /// </summary>
    [Serializable]
    public class OutputFieldZAxis : OutputFieldGrouped
    {
        /// <summary>
        /// Construct a ZAxis output field.
        /// </summary>
        /// <param name="group">The group this field belongs to.</param>
        /// <param name="field">The input field this is based on.</param>
        public OutputFieldZAxis(IOutputFieldGroup group,
                                IInputField field)
            : base(group, field)
        {
            if (!(group is ZAxisGroup))
            {
                throw new NormalizationError(
                    "Must use ZAxisGroup with OutputFieldZAxis.");
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
        /// Calculate the current value for this field. 
        /// </summary>
        /// <param name="subfield">Ignored, this field type does not have subfields.</param>
        /// <returns>The current value for this field.</returns>
        public override double Calculate(int subfield)
        {
            return (SourceField.CurrentValue*((ZAxisGroup) Group)
                                                 .Multiplier);
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }
    }
}
