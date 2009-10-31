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
