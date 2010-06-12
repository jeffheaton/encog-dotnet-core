// Encog(tm) Artificial Intelligence Framework v2.5
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
using Encog.Persist.Attributes;

namespace Encog.Normalize.Output.Nominal
{
    /// <summary>
    /// A nominal item.
    /// </summary>
    public class NominalItem
    {
        /// <summary>
        /// The low value for the range.
        /// </summary>
        [EGAttribute]
        private double low;

        /// <summary>
        /// The high value for the range.
        /// </summary>
        [EGAttribute]
        private double high;

        /// <summary>
        /// The input field used to verify against the range.
        /// </summary>
        [EGReference]
        private IInputField inputField;

        /// <summary>
        /// Construct a empty range item.  Used mainly for reflection.
        /// </summary>
        public NominalItem()
        {
        }

        /// <summary>
        /// Create a nominal item.
        /// </summary>
        /// <param name="inputField">The field that this item is based on.</param>
        /// <param name="high">The high value.</param>
        /// <param name="low">The low value.</param>
        public NominalItem(IInputField inputField, double high,
                 double low)
        {
            this.high = high;
            this.low = low;
            this.inputField = inputField;
        }

        /// <summary>
        /// Begin a row.
        /// </summary>
        public void BeginRow()
        {
        }

        /// <summary>
        /// The high value.
        /// </summary>
        public double High
        {
            get
            {
                return this.high;
            }
        }

        /// <summary>
        /// The input field value.
        /// </summary>
        public IInputField InputField
        {
            get
            {
                return this.inputField;
            }
        }

        /// <summary>
        /// The low value.
        /// </summary>
        public double Low
        {
            get
            {
                return this.low;
            }
        }

        /// <summary>
        /// Determine if the specified value is in range.
        /// </summary>
        /// <returns>True if this item is within range.</returns>
        public bool IsInRange()
        {
            double currentValue = this.inputField.CurrentValue;
            return ((currentValue >= this.low) && (currentValue <= this.high));
        }
    }
}
