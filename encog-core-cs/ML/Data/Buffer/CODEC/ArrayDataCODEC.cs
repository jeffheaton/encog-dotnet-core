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
        private double[][] _ideal;

        /// <summary>
        /// The number of ideal elements.
        /// </summary>
        private int _idealSize;

        /// <summary>
        /// The current index.
        /// </summary>
        private int _index;

        /// <summary>
        /// The input array.
        /// </summary>
        private double[][] _input;

        /// <summary>
        /// The number of input elements.
        /// </summary>
        private int _inputSize;

        /// <summary>
        /// Construct an array CODEC. 
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <param name="ideal">The ideal array.</param>
        public ArrayDataCODEC(double[][] input, double[][] ideal)
        {
            _input = input;
            _ideal = ideal;
            _inputSize = input[0].Length;
            _idealSize = ideal[0].Length;
            _index = 0;
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
            get { return _input; }
        }

        /// <inheritdoc/>
        public double[][] Ideal
        {
            get { return _ideal; }
        }

        #region IDataSetCODEC Members

        /// <inheritdoc/>
        public int InputSize
        {
            get { return _inputSize; }
        }

        /// <inheritdoc/>
        public int IdealSize
        {
            get { return _idealSize; }
        }

        /// <inheritdoc/>
        public bool Read(double[] input, double[] ideal, ref double significance)
        {
            if (_index >= _input.Length)
            {
                return false;
            }
            EngineArray.ArrayCopy(_input[_index], input);
            EngineArray.ArrayCopy(_ideal[_index], ideal);
            _index++;
            significance = 1.0;
            return true;
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal, double significance)
        {
            EngineArray.ArrayCopy(input, _input[_index]);
            EngineArray.ArrayCopy(ideal, _ideal[_index]);
            _index++;
        }

        /// <inheritdoc/>
        public void PrepareWrite(int recordCount,
                                 int inputSize, int idealSize)
        {
            _input = EngineArray.AllocateDouble2D(recordCount, inputSize);
            _ideal = EngineArray.AllocateDouble2D(recordCount, idealSize);
            _inputSize = inputSize;
            _idealSize = idealSize;
            _index = 0;
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
