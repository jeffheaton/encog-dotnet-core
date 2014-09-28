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

namespace Encog.Util.Normalize.Input
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
    [Serializable]
    public class InputFieldArray2D : BasicInputField, IHasFixedLength
    {
        /// <summary>
        /// The 2D array to use.
        /// </summary>
        private readonly double[][] _array;

        /// <summary>
        /// The 2nd dimension index to read the field from.
        /// </summary>
        private readonly int _index2;

        /// <summary>
        /// Construct a 2D array input.
        /// </summary>
        /// <param name="usedForNetworkInput">Construct a 2D array input field.</param>
        /// <param name="array">The array to use.</param>
        /// <param name="index2">index2 The secondary index to read the field from.</param>
        public InputFieldArray2D(bool usedForNetworkInput,
                                 double[][] array, int index2)
        {
            _array = array;
            _index2 = index2;
            UsedForNetworkInput = usedForNetworkInput;
        }

        #region IHasFixedLength Members

        /// <summary>
        /// The number of rows in the array.
        /// </summary>
        public int Length
        {
            get { return _array.Length; }
        }

        #endregion

        /// <summary>
        /// Gen index.
        /// </summary>
        /// <param name="i">Read a value from the specified index.</param>
        /// <returns>The value read.</returns>
        public
            override double GetValue(int i)
        {
            return _array[i][_index2];
        }
    }
}
