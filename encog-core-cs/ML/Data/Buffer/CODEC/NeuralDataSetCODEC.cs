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
using System.Collections.Generic;
using Encog.ML.Data.Basic;
using Encog.Util;
using System;

namespace Encog.ML.Data.Buffer.CODEC
{
    /// <summary>
    /// A CODEC that works with the NeuralDataSet class.
    /// </summary>
    public class NeuralDataSetCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The dataset.
        /// </summary>
        private readonly IMLDataSet _dataset;

        /// <summary>
        /// The iterator used to read through the dataset.
        /// </summary>
        private IEnumerator<IMLDataPair> _enumerator;

        /// <summary>
        /// The number of ideal elements.
        /// </summary>
        private int _idealSize;

        /// <summary>
        /// The number of input elements.
        /// </summary>
        private int _inputSize;

        /// <summary>
        /// Construct a CODEC. 
        /// </summary>
        /// <param name="dataset">The dataset to use.</param>
        public NeuralDataSetCODEC(IMLDataSet dataset)
        {
            _dataset = dataset;
            _inputSize = dataset.InputSize;
            _idealSize = dataset.IdealSize;
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
            if (!_enumerator.MoveNext())
            {
                return false;
            }
            else
            {
                IMLDataPair pair = _enumerator.Current;
				pair.Input.CopyTo(input, 0, pair.Input.Count);
				pair.Ideal.CopyTo(ideal, 0, pair.Ideal.Count);
                significance = pair.Significance;
                return true;
            }
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal, double significance)
        {
			throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void PrepareWrite(int recordCount,
                                 int inputSize, int idealSize)
        {
            _inputSize = inputSize;
            _idealSize = idealSize;
        }

        /// <inheritdoc/>
        public void PrepareRead()
        {
            _enumerator = _dataset.GetEnumerator();
        }

        /// <inheritdoc/>
        public void Close()
        {
        }

        #endregion
    }
}
