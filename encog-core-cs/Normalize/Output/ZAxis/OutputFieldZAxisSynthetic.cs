// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Normalize.Output.ZAxis
{
    /// <summary>
    /// This field represents the synthetic value used in Z-Axis normalization.
    /// For more information see the OutputFieldZAxis class.
    /// </summary>
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
        /// Calculate the synthetic value for this Z-Axis normalization.
        /// </summary>
        /// <param name="subfield">Not used.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            double l = ((ZAxisGroup)Group).Length;
            double f = ((ZAxisGroup)Group).Multiplier;
            double n = Group.GroupedFields.Count;
            double result = f * Math.Sqrt(n - (l * l));
            if (double.IsInfinity(result) || double.IsNaN(result))
            {
                return 0;
            }
            else
            {
                return result;
            }
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
