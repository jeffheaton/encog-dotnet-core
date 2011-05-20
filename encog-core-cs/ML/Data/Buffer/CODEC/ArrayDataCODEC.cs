//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.Util;

namespace Encog.ML.Data.Buffer.CODEC
{
    /// <summary>
    /// A CODEC used for arrays.
    /// </summary>
    public class ArrayDataCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The ideal array.
        /// </summary>
        private double[][] ideal;

        /// <summary>
        /// The number of ideal elements.
        /// </summary>
        private int idealSize;

        /// <summary>
        /// The current index.
        /// </summary>
        private int index;

        /// <summary>
        /// The input array.
        /// </summary>
        private double[][] input;

        /// <summary>
        /// The number of input elements.
        /// </summary>
        private int inputSize;

        /// <summary>
        /// Construct an array CODEC. 
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <param name="ideal">The ideal array.</param>
        public ArrayDataCODEC(double[][] input, double[][] ideal)
        {
            this.input = input;
            this.ideal = ideal;
            inputSize = input[0].Length;
            idealSize = ideal[0].Length;
            index = 0;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ArrayDataCODEC()
        {
        }

        /// <inheritdoc/>
        public double[][] Input
        {
            get { return input; }
        }

        /// <inheritdoc/>
        public double[][] Ideal
        {
            get { return ideal; }
        }

        #region IDataSetCODEC Members

        /// <inheritdoc/>
        public int InputSize
        {
            get { return inputSize; }
        }

        /// <inheritdoc/>
        public int IdealSize
        {
            get { return idealSize; }
        }

        /// <inheritdoc/>
        public bool Read(double[] input, double[] ideal)
        {
            if (index >= this.input.Length)
            {
                return false;
            }
            else
            {
                EngineArray.ArrayCopy(this.input[index], input);
                EngineArray.ArrayCopy(this.ideal[index], ideal);
                index++;
                return true;
            }
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal)
        {
            EngineArray.ArrayCopy(input, this.input[index]);
            EngineArray.ArrayCopy(ideal, this.ideal[index]);
            index++;
        }

        /// <inheritdoc/>
        public void PrepareWrite(int recordCount,
                                 int inputSize, int idealSize)
        {
            input = EngineArray.AllocateDouble2D(recordCount, inputSize);
            ideal = EngineArray.AllocateDouble2D(recordCount, idealSize);
            this.inputSize = inputSize;
            this.idealSize = idealSize;
            index = 0;
        }

        /// <inheritdoc/>
        public void PrepareRead()
        {
        }

        /// <inheritdoc/>
        public void Close()
        {
        }

        #endregion
    }
}
