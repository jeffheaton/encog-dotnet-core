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

namespace Encog.Normalize.Input
{
    /// <summary>
    /// An input field that comes from a 1D array.
    /// 
    /// Note: this input field will not be persisted to an EG file.
    /// This is because it could point to a lengthy array, that really
    /// has no meaning inside of an EG file. 
    /// </summary>
    [EGUnsupported]
    public class InputFieldArray1D : BasicInputField, IHasFixedLength
    {
        /// <summary>
        /// A reference to the array.
        /// </summary>
        private double[] array;

        /// <summary>
        /// Construct the 1D array field.
        /// </summary>
        /// <param name="usedForNetworkInput">True if this field is used for the actual
        /// input to the neural network.  See getUsedForNetworkInput for more info.</param>
        /// <param name="array">The array to use.</param>
        public InputFieldArray1D(bool usedForNetworkInput,
                double[] array)
        {
            this.array = array;
            UsedForNetworkInput = usedForNetworkInput;
        }

        /// <summary>
        /// Get the value from the specified index.
        /// </summary>
        /// <param name="i">The index to retrieve.</param>
        /// <returns>The value at the specified index.</returns>
        public override double GetValue(int i)
        {
            return this.array[i];
        }

        /// <summary>
        /// The length of the array.
        /// </summary>
        public int Length
        {
            get
            {
                return this.array.Length;
            }
        }
    }
}
