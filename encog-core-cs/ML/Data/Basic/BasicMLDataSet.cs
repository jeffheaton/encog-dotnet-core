// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

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
#if !SILVERLIGHT
    [Serializable]
#endif
    public class BasicMLDataSet : MLDataSet, IEnumerable<MLDataPair>
    {
        /// <summary>
        /// The enumerator for the basic neural data set.
        /// </summary>
#if !SILVERLIGHT
        [Serializable]
#endif
        public class BasicNeuralEnumerator : IEnumerator<MLDataPair>
        {
            /// <summary>
            /// The current index.
            /// </summary>
            private int current;

            /// <summary>
            /// The owner.
            /// </summary>
            private readonly BasicMLDataSet owner;

            /// <summary>
            /// Construct an enumerator.
            /// </summary>
            /// <param name="owner">The owner of the enumerator.</param>
            public BasicNeuralEnumerator(BasicMLDataSet owner)
            {
                current = -1;
                this.owner = owner;
            }

            /// <summary>
            /// The current data item.
            /// </summary>
            public MLDataPair Current
            {
                get { return owner.data[current]; }
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
                    if (current < 0)
                    {
                        throw new InvalidOperationException("Must call MoveNext before reading Current.");
                    }
                    return owner.data[current];
                }
            }

            /// <summary>
            /// Move to the next item.
            /// </summary>
            /// <returns>True if there is a next item.</returns>
            public bool MoveNext()
            {
                current++;
                if (current >= owner.data.Count)
                    return false;
                return true;
            }

            /// <summary>
            /// Reset to the beginning.
            /// </summary>
            public void Reset()
            {
                current = -1;
            }
        }

        /// <summary>
        /// Access to the list of data items.
        /// </summary>
        public virtual IList<MLDataPair> Data
        {
            get { return data; }
            set { data = value; }
        }


        /// <summary>
        /// The data held by this object.
        /// </summary>
        private IList<MLDataPair> data = new List<MLDataPair>();

        /// <summary>
        /// The enumerators created for this list.
        /// </summary>
        private IList<BasicNeuralEnumerator> enumerators =
            new List<BasicNeuralEnumerator>();

        /// <summary>
        /// Construct a data set from an already created list. Mostly used to
        /// duplicate this class.
        /// </summary>
        /// <param name="data">The data to use.</param>
        public BasicMLDataSet(IList<MLDataPair> data)
        {
            this.data = data;
        }

        /// <summary>
        /// Copy whatever dataset type is specified into a memory dataset.
        /// </summary>
        ///
        /// <param name="set">The dataset to copy.</param>
        public BasicMLDataSet(MLDataSet set)
        {
            data = new List<MLDataPair>();
            int inputCount = set.InputSize;
            int idealCount = set.IdealSize;


            foreach (MLDataPair pair in set)
            {
                BasicMLData input = null;
                BasicMLData ideal = null;

                if (inputCount > 0)
                {
                    input = new BasicMLData(inputCount);
                    EngineArray.ArrayCopy(pair.InputArray, input.Data);
                }

                if (idealCount > 0)
                {
                    ideal = new BasicMLData(idealCount);
                    EngineArray.ArrayCopy(pair.IdealArray, ideal.Data);
                }

                Add(new BasicMLDataPair(input, ideal));
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
                double[] tempIdeal = null;

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
                if (data == null || data.Count == 0)
                    return 0;
                MLDataPair pair = data[0];
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
                if (data == null || data.Count == 0)
                    return 0;
                MLDataPair pair = data[0];
                return pair.Input.Count;
            }
        }

        /// <summary>
        /// Add the specified data to the set.  Add unsupervised data.
        /// </summary>
        /// <param name="data1">The data to add to the set.</param>
        public virtual void Add(MLData data1)
        {
            MLDataPair pair = new BasicMLDataPair(data1, null);
            data.Add(pair);
        }

        /// <summary>
        /// Add supervised data to the set.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <param name="idealData">The ideal data.</param>
        public virtual void Add(MLData inputData, MLData idealData)
        {
            MLDataPair pair = new BasicMLDataPair(inputData, idealData);
            data.Add(pair);
        }

        /// <summary>
        /// Add a pair to the set.
        /// </summary>
        /// <param name="inputData">The pair to add to the set.</param>
        public virtual void Add(MLDataPair inputData)
        {
            data.Add(inputData);
        }

        /// <summary>
        /// Close the neural data set.
        /// </summary>
        public virtual void Close()
        {
            // not needed
        }

        /// <summary>
        /// Get an enumerator to access the data with.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<MLDataPair> GetEnumerator()
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
                if (data.Count == 0)
                    return false;
                return (data[0].Supervised);
            }
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public object Clone()
        {
            var result = new BasicMLDataSet();
            foreach (MLDataPair pair in Data)
            {
                result.Add((MLDataPair) pair.Clone());
            }
            return result;
        }

        /// <summary>
        /// The number of records in this data set.
        /// </summary>
        public long Count
        {
            get { return data.Count; }
        }

        /// <summary>
        /// Get one record from the data set.
        /// </summary>
        /// <param name="index">The index to read.</param>
        /// <param name="pair">The pair to read into.</param>
        public void GetRecord(long index, MLDataPair pair)
        {
            MLDataPair source = data[(int) index];
            pair.InputArray = source.Input.Data;
            if (pair.IdealArray != null)
            {
                pair.IdealArray = source.Ideal.Data;
            }
        }

        /// <summary>
        /// Open an additional instance of this dataset.
        /// </summary>
        /// <returns>The new instance of this dataset.</returns>
        public MLDataSet OpenAdditional()
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
                if (data.Count == 0)
                {
                    return false;
                }
                else
                {
                    return data[0].Supervised;
                }
            }
        }
    }
}