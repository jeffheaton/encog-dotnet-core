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
using System.Collections;
using System.Collections.Generic;
using Encog.ML.Data.Basic;
using Encog.Util;

namespace Encog.ML.Data.Versatile
{
    /// <summary>
    ///     The MatrixMLDataSet can use a large 2D matrix of doubles to internally hold
    ///     data. It supports several advanced features such as the ability to mask and
    ///     time-box. Masking allows several datasets to use the same backing array,
    ///     however use different parts.
    ///     Time boxing allows time-series data to be represented for prediction. The
    ///     following shows how data is laid out for different lag and lead settings.
    ///     Lag 0; Lead 0 [10 rows] 1->1 2->2 3->3 4->4 5->5 6->6 7->7 8->8 9->9 10->10
    ///     Lag 0; Lead 1 [9 rows] 1->2 2->3 3->4 4->5 5->6 6->7 7->8 8->9 9->10
    ///     Lag 1; Lead 0 [9 rows, not useful] 1,2->1 2,3->2 3,4->3 4,5->4 5,6->5 6,7->6
    ///     7,8->7 8,9->8 9,10->9
    ///     Lag 1; Lead 1 [8 rows] 1,2->3 2,3->4 3,4->5 4,5->6 5,6->7 6,7->8 7,8->9
    ///     8,9->10
    ///     Lag 1; Lead 2 [7 rows] 1,2->3,4 2,3->4,5 3,4->5,6 4,5->6,7 5,6->7,8 6,7->8,9
    ///     7,8->9,10
    ///     Lag 2; Lead 1 [7 rows] 1,2,3->4 2,3,4->5 3,4,5->6 4,5,6->7 5,6,7->8 6,7,8->9
    ///     7,8,9->10
    /// </summary>
    [Serializable]
    public class MatrixMLDataSet : IMLDataSetAddable, IEnumerable<IMLDataPair>
    {
        /// <summary>
        ///     The mask to the data.
        /// </summary>
        private readonly int[] _mask;

        /// <summary>
        ///     The default constructor.
        /// </summary>
        public MatrixMLDataSet()
        {
            CalculatedInputSize = -1;
            CalculatedIdealSize = -1;
        }

        /// <summary>
        ///     Construct the dataset with no mask.
        /// </summary>
        /// <param name="theData">The backing array.</param>
        /// <param name="theCalculatedInputSize">The input size.</param>
        /// <param name="theCalculatedIdealSize">The ideal size.</param>
        public MatrixMLDataSet(double[][] theData, int theCalculatedInputSize,
            int theCalculatedIdealSize) : this()
        {
            Data = theData;
            CalculatedInputSize = theCalculatedInputSize;
            CalculatedIdealSize = theCalculatedIdealSize;
        }

        /// <summary>
        ///     Construct the dataset from a 2D double array.
        /// </summary>
        /// <param name="theData">The data.</param>
        /// <param name="inputCount">The input count.</param>
        /// <param name="idealCount">The ideal count.</param>
        /// <param name="theMask">The mask.</param>
        public MatrixMLDataSet(double[][] theData, int inputCount, int idealCount,
            int[] theMask) : this()
        {
            Data = theData;
            CalculatedInputSize = inputCount;
            CalculatedIdealSize = idealCount;
            _mask = theMask;
        }

        /// <summary>
        ///     Construct the dataset from another matrix dataset.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="mask">The mask.</param>
        public MatrixMLDataSet(MatrixMLDataSet data, int[] mask) : this()
        {
            Data = data.Data;
            CalculatedInputSize = data.CalculatedInputSize;
            CalculatedIdealSize = data.CalculatedIdealSize;
            _mask = mask;
        }

        /// <summary>
        ///     The number of inputs.
        /// </summary>
        public int CalculatedInputSize { get; set; }

        /// <summary>
        ///     The number of ideal values.
        /// </summary>
        public int CalculatedIdealSize { get; set; }

        /// <summary>
        ///     The backing data.
        /// </summary>
        public double[][] Data { get; set; }

        /// <summary>
        ///     The lag window size.
        /// </summary>
        public int LagWindowSize { get; set; }

        /// <summary>
        ///     The lead window size.
        /// </summary>
        public int LeadWindowSize { get; set; }

        /// <summary>
        /// The mask.
        /// </summary>
        public int[] Mask
        {
            get { return _mask; }
        }

        /// <summary>
        /// Create an enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MatrixMLDataSetEnumerator(this);
        }

        /// <summary>
        /// Create an enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator<IMLDataPair> IEnumerable<IMLDataPair>.GetEnumerator()
        {
            return new MatrixMLDataSetEnumerator(this);
        }


        /// <inheritdoc />
        public int IdealSize
        {
            get { return CalculatedIdealSize*Math.Min(LeadWindowSize, 1); }
        }

        /// <inheritdoc />
        public int InputSize
        {
            get { return CalculatedInputSize*LagWindowSize; }
        }

        /// <inheritdoc />
        public bool Supervised
        {
            get { return IdealSize == 0; }
        }

        /// <inheritdoc />
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            return new MatrixMLDataSetEnumerator(this);
        }

        /// <inheritdoc />
        public IMLDataSet OpenAdditional()
        {
            var result = new MatrixMLDataSet(Data,
                CalculatedInputSize, CalculatedIdealSize, _mask)
            {
                LagWindowSize = LagWindowSize,
                LeadWindowSize = LeadWindowSize
            };
            return result;
        }

        /// <inheritdoc />
        public void Add(IMLData data1)
        {
            // TODO Auto-generated method stub
        }

        /// <inheritdoc />
        public void Add(IMLData inputData, IMLData idealData)
        {
            // TODO Auto-generated method stub
        }

        /// <inheritdoc />
        public void Add(IMLDataPair inputData)
        {
            // TODO Auto-generated method stub
        }

        /// <inheritdoc />
        public void Close()
        {
            // TODO Auto-generated method stub
        }

        /// <inheritdoc />
        public int Count
        {
            get { return (int) GetRecordCount(); }
        }

        /// <inheritdoc />
        public IMLDataPair this[int index]
        {
            get
            {
                if (index > Count)
                {
                    return null;
                }

                var input = new BasicMLData(
                    CalculatedInputSize*CalculateLagCount());
                var ideal = new BasicMLData(
                    CalculatedIdealSize*CalculateLeadCount());
                IMLDataPair pair = new BasicMLDataPair(input, ideal);

                GetRecord(index, pair);

                return pair;
            }
        }

        /// <inheritdoc />
        public long GetRecordCount()
        {
            if (Data == null)
            {
                throw new EncogError(
                    "You must normalize the dataset before using it.");
            }

            if (_mask == null)
            {
                return Data.Length
                       - (LagWindowSize + LeadWindowSize);
            }
            return _mask.Length - (LagWindowSize + LeadWindowSize);
        }

        /// <summary>
        /// Calculate the actual lead count.
        /// </summary>
        /// <returns>The actual lead count.</returns>
        private int CalculateLagCount()
        {
            return (LagWindowSize <= 0)
                ? 1
                : (LagWindowSize + 1);
        }

        /// <summary>
        /// Calculate the actual lead count.
        /// </summary>
        /// <returns>The actual lead count.</returns>
        private int CalculateLeadCount()
        {
            return (LeadWindowSize <= 1) ? 1 : LeadWindowSize;
        }

        /// <inheritdoc />
        public void GetRecord(long index, IMLDataPair pair)
        {
            if (Data == null)
            {
                throw new EncogError(
                    "You must normalize the dataset before using it.");
            }

            double[] inputArray = ((BasicMLData) pair.Input).Data;
            double[] idealArray = ((BasicMLData) pair.Ideal).Data;

            // Copy the input, account for time windows.
            int inputSize = CalculateLagCount();
            for (int i = 0; i < inputSize; i++)
            {
                double[] dataRow = LookupDataRow((int) (index + i));

                EngineArray.ArrayCopy(dataRow, 0, inputArray, i
                                                              *CalculatedInputSize,
                    CalculatedInputSize);
            }

            // Copy the output, account for time windows.
            int outputStart = (LeadWindowSize > 0) ? 1 : 0;
            int outputSize = CalculateLeadCount();
            for (int i = 0; i < outputSize; i++)
            {
                double[] dataRow = LookupDataRow((int) (index + i + outputStart));
                EngineArray.ArrayCopy(dataRow, CalculatedInputSize,
                    idealArray, i
                                *CalculatedIdealSize,
                    CalculatedIdealSize);
            }
        }

        /// <summary>
        /// Find a row, using the mask.
        /// </summary>
        /// <param name="index">The index we seek.</param>
        /// <returns>The row.</returns>
        private double[] LookupDataRow(int index)
        {
            if (_mask != null)
            {
                return Data[_mask[index]];
            }
            return Data[index];
        }

        /// <summary>
        ///     The enumerator for the matrix data set.
        /// </summary>
        [Serializable]
        public class MatrixMLDataSetEnumerator : IEnumerator<IMLDataPair>
        {
            /// <summary>
            ///     The owner.
            /// </summary>
            private readonly MatrixMLDataSet _owner;

            /// <summary>
            ///     The current index.
            /// </summary>
            private int _current;

            /// <summary>
            ///     Construct an enumerator.
            /// </summary>
            /// <param name="owner">The owner of the enumerator.</param>
            public MatrixMLDataSetEnumerator(MatrixMLDataSet owner)
            {
                _current = -1;
                _owner = owner;
            }

            /// <summary>
            ///     The current data item.
            /// </summary>
            public IMLDataPair Current
            {
                get { return InternalCurrent(); }
            }

            /// <summary>
            ///     Dispose of this object.
            /// </summary>
            public void Dispose()
            {
                // nothing needed
            }

            /// <summary>
            ///     The current item.
            /// </summary>
            object IEnumerator.Current
            {
                get { return InternalCurrent(); }
            }

            /// <summary>
            ///     Move to the next item.
            /// </summary>
            /// <returns>True if there is a next item.</returns>
            public bool MoveNext()
            {
                _current++;
                if (_current >= _owner.Count)
                    return false;
                return true;
            }

            /// <summary>
            ///     Reset to the beginning.
            /// </summary>
            public void Reset()
            {
                _current = -1;
            }

            private IMLDataPair InternalCurrent()
            {
                if (_current < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }

                return _owner[_current];
            }
        }
    }
}
