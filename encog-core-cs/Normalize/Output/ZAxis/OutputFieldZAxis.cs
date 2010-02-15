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

namespace Encog.Normalize.Output.ZAxis
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
        /// Calculate the current value for this field. 
        /// </summary>
        /// <param name="subfield">Ignored, this field type does not have subfields.</param>
        /// <returns>The current value for this field.</returns>
        public override double Calculate(int subfield)
        {
            return (SourceField.CurrentValue * ((ZAxisGroup)Group)
                    .Multiplier);
        }

        /// <summary>
        /// The subfield count, which is one, as this field type does not
        /// have subfields.
        /// </summary>
        public override int SubfieldCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }

    }
}
