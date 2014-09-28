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
    /// An input field that comes from a 1D array.
    /// 
    /// Note: this input field will not be persisted to an EG file.
    /// This is because it could point to a lengthy array, that really
    /// has no meaning inside of an EG file. 
    /// </summary>
    [Serializable]
    public class InputFieldArray1D : BasicInputField, IHasFixedLength
    {
        /// <summary>
        /// A reference to the array.
        /// </summary>
        private readonly double[] _array;

        /// <summary>
        /// Construct the 1D array field.
        /// </summary>
        /// <param name="usedForNetworkInput">True if this field is used for the actual
        /// input to the neural network.  See getUsedForNetworkInput for more info.</param>
        /// <param name="array">The array to use.</param>
        public InputFieldArray1D(bool usedForNetworkInput,
                                 double[] array)
        {
            _array = array;
            UsedForNetworkInput = usedForNetworkInput;
        }

        #region IHasFixedLength Members

        /// <summary>
        /// The length of the array.
        /// </summary>
        public int Length
        {
            get { return _array.Length; }
        }

        #endregion

        /// <summary>
        /// Get the value from the specified index.
        /// </summary>
        /// <param name="i">The index to retrieve.</param>
        /// <returns>The value at the specified index.</returns>
        public override double GetValue(int i)
        {
            return _array[i];
        }
    }
}
