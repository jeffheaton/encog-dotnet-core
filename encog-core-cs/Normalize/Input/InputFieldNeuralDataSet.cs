using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using Encog.Persist.Attributes;

namespace Encog.Normalize.Input
{
    /// <summary>
    /// An input field based on an Encog NeuralDataSet.
    /// </summary>
    [EGUnsupported]
    public class InputFieldNeuralDataSet : BasicInputField
    {
        /// <summary>
        /// The data set.
        /// </summary>
        private INeuralDataSet data;

        /// <summary>
        /// The input or ideal index.  This treats the input and ideal as one
        /// long array, concatenated together.
        /// </summary>
        private int offset;

        /// <summary>
        /// Construct a input field based on a NeuralDataSet.
        /// </summary>
        /// <param name="usedForNetworkInput">Is this field used for neural input.</param>
        /// <param name="data">The data set to use.</param>
        /// <param name="offset">The input or ideal index to use. This treats the input 
        /// and ideal as one long array, concatenated together.</param>
        public InputFieldNeuralDataSet(bool usedForNetworkInput,
                 INeuralDataSet data, int offset)
        {
            this.data = data;
            this.offset = offset;
            UsedForNetworkInput = usedForNetworkInput;
        }

        /// <summary>
        /// The neural data set to read.
        /// </summary>
        public INeuralDataSet NeuralDataSet
        {
            get
            {
                return this.data;
            }
        }

        /// <summary>
        /// The field to be accessed. This treats the input and 
        /// ideal as one long array, concatenated together.
        /// </summary>
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

    }
}
