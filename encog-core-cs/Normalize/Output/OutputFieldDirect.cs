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
using Encog.Persist.Attributes;
using Encog.Normalize.Input;

namespace Encog.Normalize.Output
{
    /// <summary>
    /// A direct output field, will simply pass the input value to the output.
    /// </summary>
    public class OutputFieldDirect : BasicOutputField
    {
        /// <summary>
        /// The source field.
        /// </summary>
        [EGReference]
        private IInputField sourceField;


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
            this.sourceField = sourceField;
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
            return this.sourceField.CurrentValue;
        }

        /// <summary>
        /// Always returns 1, as subfields are not used.
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
