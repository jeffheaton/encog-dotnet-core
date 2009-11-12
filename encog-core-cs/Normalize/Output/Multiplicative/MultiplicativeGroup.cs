
// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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

namespace Encog.Normalize.Output.Multiplicative
{
    /// <summary>
    /// Used to group multiplicative fields together.
    /// </summary>
    public class MultiplicativeGroup : BasicOutputFieldGroup
    {
        /// <summary>
        /// The "length" of this field.
        /// </summary>
        private double length;

        /// <summary>
        /// The length of this field.  This is the sum of the squares of
        /// all of the groupped fields.  The square root of this sum is the 
        /// length. 
        /// </summary>
        public double Length
        {
            get
            {
                return this.length;
            }
        }

        /// <summary>
        /// Called to init this group for a new field.  This recalculates the
        /// "length".
        /// </summary>
        public override void RowInit()
        {
            double value = 0;

            foreach (OutputFieldGrouped field in this.GroupedFields)
            {
                value += (field.SourceField.CurrentValue * field
                        .SourceField.CurrentValue);
            }
            this.length = Math.Sqrt(value);
        }

    }
}
