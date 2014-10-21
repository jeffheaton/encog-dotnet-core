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
using Encog.Util;

namespace Encog.ML.Data.Basic
{
    /// <summary>
    /// Basic implementation of the NeuralDataSet class.  This class simply
    /// stores the neural data in an ArrayList.  This class is memory based, 
    /// so large enough datasets could cause memory issues.  Many other dataset
    /// types extend this class.
    /// </summary>
    [Serializable]
    public class BasicMLDataSet : IMLDataSetAddable, IEnumerable<IMLDataPair>
    {
        /// <summary>
        /// The enumerator for the basic neural data set.
        /// </summary>
        [Serializable]
        public class BasicNeuralEnumerator : IEnumerator<IMLDataPair>
        {
            /// <summary>
            /// The current index.
            /// </summary>
            private int _current;

            /// <summary>
            /// The owner.
            /// </summary>
            private readonly BasicMLDataSet _owner;

            /// <summary>
            /// Construct an enumerator.
            /// </summary>
            /// <param name="owner">The owner of the enumerator.</param>
            public BasicNeuralEnumerator(BasicMLDataSet owner)
            {
                _current = -1;
                _owner = owner;
            }

            /// <summary>
            /// The current data item.
            /// </summary>
            public IMLDataPair Current
            {
                get { return _owner._data[_current]; }
            }

            /// <summary>
            /// Dispose of this object.
            /// </summary>
            public void Dispose()
            {
                // nothing needed
            }

            /// <summary>
            /// The current item.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (_current < 0)
                    {
                        throw new InvalidOperationException("Must call MoveNext before reading Current.");
                    }
                    return _owner._data[_current];
                }
            }

            /// <summary>
            /// Move to the next item.
            /// </summary>
            /// <returns>True if there is a next item.</returns>
            public bool MoveNext()
            {
                _current++;
                if (_current >= _owner._data.Count)
                    return false;
                return true;
            }

            /// <summary>
            /// Reset to the beginning.
            /// </summary>
            public void Reset()
            {
                _current = -1;
            }
        }

        /// <summary>
        /// Access to the list of data items.
        /// </summary>
        public IList<IMLDataPair> Data
        {
            get { return _data; }
            set { _data = value; }
        }


        /// <summary>
        /// The data held by this object.
        /// </summary>
        private IList<IMLDataPair> _data = new List<IMLDataPair>();

        /// <summary>
        /// Construct a data set from an already created list. Mostly used to
        /// duplicate this class.
        /// </summary>
        /// <param name="data">The data to use.</param>
        public BasicMLDataSet(IList<IMLDataPair> data)
        {
            _data = data;
        }

        /// <summary>
        /// Copy whatever dataset type is specified into a memory dataset.
        /// </summary>
        ///
        /// <param name="set">The dataset to copy.</param>
        public BasicMLDataSet(IMLDataSet set)
        {
            _data = new List<IMLDataPair>();
            int inputCount = set.InputSize;
            int idealCount = set.IdealSize;


            foreach (IMLDataPair pair in set)
            {
                BasicMLData input = null;
                BasicMLData ideal = null;

                if (inputCount > 0)
                {
                    input = new BasicMLData(inputCount);
					pair.Input.CopyTo(input.Data, 0, pair.Input.Count);
                }

                if (idealCount > 0)
                {
                    ideal = new BasicMLData(idealCount);
					pair.Ideal.CopyTo(ideal.Data, 0, pair.Ideal.Count);
                }

                Add(new BasicMLDataPair(input, ideal)); // should we copy Significance here?
            }
        }


        /// <summary>
        /// Construct a data set from an input and idea array.
        /// </summary>
        /// <param name="input">The input into the neural network for training.</param>
        /// <param name="ideal">The idea into the neural network for training.</param>
        public BasicMLDataSet(double[][] input, double[][] ideal)
        {
            for (int i = 0; i < input.Length; i++)
            {
                var tempInput = new double[input[0].Length];
                double[] tempIdeal;

                for (int j = 0; j < tempInput.Length; j++)
                {
                    tempInput[j] = input[i][j];
                }

                BasicMLData idealData = null;

                if (ideal != null)
                {
                    tempIdeal = new double[ideal[0].Length];
                    for (int j = 0; j < tempIdeal.Length; j++)
                    {
                        tempIdeal[j] = ideal[i][j];
                    }
                    idealData = new BasicMLData(tempIdeal);
                }

                var inputData = new BasicMLData(tempInput);

                Add(inputData, idealData);
            }
        }

        /// <summary>
        /// Construct a basic neural data set.
        /// </summary>
        public BasicMLDataSet()
        {
        }

        /// <summary>
        /// Get the ideal size, or zero for unsupervised.
        /// </summary>
        public virtual int IdealSize
        {
            get
            {
                if (_data == null || _data.Count <= 0)
                {
                    return 0;
                }

                IMLDataPair pair = _data[0];

                if (pair.Ideal == null)
                {
                    return 0;
                }

                return pair.Ideal.Count;
            }
        }

        /// <summary>
        /// Get the input size.
        /// </summary>
        public virtual int InputSize
        {
            get
            {
                if (_data == null || _data.Count == 0)
                    return 0;
                IMLDataPair pair = _data[0];
                return pair.Input.Count;
            }
        }

        /// <summary>
        /// Add the specified data to the set.  Add unsupervised data.
        /// </summary>
        /// <param name="data1">The data to add to the set.</param>
        public virtual void Add(IMLData data1)
        {
            IMLDataPair pair = new BasicMLDataPair(data1, null);
            _data.Add(pair);
        }

        /// <summary>
        /// Add supervised data to the set.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <param name="idealData">The ideal data.</param>
        public virtual void Add(IMLData inputData, IMLData idealData)
        {
            IMLDataPair pair = new BasicMLDataPair(inputData, idealData);
            _data.Add(pair);
        }

        /// <summary>
        /// Add a pair to the set.
        /// </summary>
        /// <param name="inputData">The pair to add to the set.</param>
        public virtual void Add(IMLDataPair inputData)
        {
            _data.Add(inputData);
        }

        /// <summary>
        /// Close the neural data set.
        /// </summary>
        public void Close()
        {
            // not needed
        }

        /// <summary>
        /// Get an enumerator to access the data with.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            return new BasicNeuralEnumerator(this);
        }

        /// <summary>
        /// Get an enumerator to access the data with.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BasicNeuralEnumerator(this);
        }

        /// <summary>
        /// Determine if the dataset is supervised.  It is assumed that all pairs
        /// are either supervised or not.  So we can determine the entire set by
        /// looking at the first item.  If the set is empty then return false, or
        /// unsupervised.
        /// </summary>
        public bool IsSupervised
        {
            get
            {
                if (_data.Count == 0)
                    return false;
                return (_data[0].Supervised);
            }
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public object Clone()
        {
            var result = new BasicMLDataSet();
            foreach (IMLDataPair pair in Data)
            {
                result.Add((IMLDataPair) pair.Clone());
            }
            return result;
        }

        /// <summary>
        /// The number of records in this data set.
        /// </summary>
        public int Count
        {
            get { return _data.Count; }
        }

        /// <summary>
        /// Open an additional instance of this dataset.
        /// </summary>
        /// <returns>The new instance of this dataset.</returns>
        public IMLDataSet OpenAdditional()
        {
            return new BasicMLDataSet(Data);
        }


        /// <summary>
        /// Return true if supervised.
        /// </summary>
        public bool Supervised
        {
            get
            {
                if (_data.Count == 0)
                {
                    return false;
                }
                return _data[0].Supervised;
            }
        }

        public IMLDataPair this[int x]
        {
            get { return _data[x]; }
        }
    }
}
