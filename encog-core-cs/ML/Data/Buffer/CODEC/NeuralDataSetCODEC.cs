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
using System.Collections.Generic;
using Encog.ML.Data.Basic;
using Encog.Util;

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
        private readonly MLDataSet dataset;

        /// <summary>
        /// The iterator used to read through the dataset.
        /// </summary>
        private IEnumerator<MLDataPair> enumerator;

        /// <summary>
        /// The number of ideal elements.
        /// </summary>
        private int idealSize;

        /// <summary>
        /// The number of input elements.
        /// </summary>
        private int inputSize;

        /// <summary>
        /// Construct a CODEC. 
        /// </summary>
        /// <param name="dataset">The dataset to use.</param>
        public NeuralDataSetCODEC(MLDataSet dataset)
        {
            this.dataset = dataset;
            inputSize = dataset.InputSize;
            idealSize = dataset.IdealSize;
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
            if (!enumerator.MoveNext())
            {
                return false;
            }
            else
            {
                MLDataPair pair = enumerator.Current;
                EngineArray.ArrayCopy(pair.Input.Data, input);
                EngineArray.ArrayCopy(pair.Ideal.Data, ideal);
                return true;
            }
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal)
        {
            MLDataPair pair = BasicMLDataPair.CreatePair(inputSize,
                                                         idealSize);
            EngineArray.ArrayCopy(input, pair.Input.Data);
            EngineArray.ArrayCopy(ideal, pair.Ideal.Data);
        }

        /// <inheritdoc/>
        public void PrepareWrite(int recordCount,
                                 int inputSize, int idealSize)
        {
            this.inputSize = inputSize;
            this.idealSize = idealSize;
        }

        /// <inheritdoc/>
        public void PrepareRead()
        {
            enumerator = dataset.GetEnumerator();
        }

        /// <inheritdoc/>
        public void Close()
        {
        }

        #endregion
    }
}
