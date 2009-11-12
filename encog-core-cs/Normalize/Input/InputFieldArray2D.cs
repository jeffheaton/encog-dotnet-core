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
using Encog.Persist.Attributes;

namespace Encog.Normalize.Input
{
    /// <summary>
    ///  * An input field that comes from a 2D array. The first dimension
    /// of the array will be used to read each successive row.  The second
    /// dimension is fixed, and specified in the constructor.  You would create
    /// multiple InputFieldArray2D object to read each of the "columns" stored
    /// at each row.
    /// 
    /// Note: this input field will not be persisted to an EG file.
    /// This is because it could point to a lengthy array, that really
    /// has no meaning inside of an EG file.  
    /// </summary>
    [EGUnsupported]
    public class InputFieldArray2D : BasicInputField, IHasFixedLength
    {
        /// <summary>
        /// The 2D array to use.
        /// </summary>
        private double[][] array;

        /// <summary>
        /// The 2nd dimension index to read the field from.
        /// </summary>
        private int index2;

        /// <summary>
        /// Construct a 2D array input.
        /// </summary>
        /// <param name="usedForNetworkInput">Construct a 2D array input field.</param>
        /// <param name="array">The array to use.</param>
        /// <param name="index2">index2 The secondary index to read the field from.</param>
        public InputFieldArray2D(bool usedForNetworkInput,
                 double[][] array, int index2)
        {
            this.array = array;
            this.index2 = index2;
            UsedForNetworkInput = usedForNetworkInput;
        }


        /// <summary>
        /// Gen index.
        /// </summary>
        /// <param name="i">Read a value from the specified index.</param>
        /// <returns>The value read.</returns>
        override
        public double GetValue(int i)
        {
            return this.array[i][this.index2];
        }

        /// <summary>
        /// The number of rows in the array.
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
