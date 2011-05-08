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
using System.Collections.Generic;
using Encog.Persist;
using Encog.Persist.Persistors;

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
    public class BasicMLDataSet : BasicPersistedObject, MLDataSet, IEnumerable<MLDataPair>, IEncogPersistedObject
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
            private BasicMLDataSet owner;

            /// <summary>
            /// Construct an enumerator.
            /// </summary>
            /// <param name="owner">The owner of the enumerator.</param>
            public BasicNeuralEnumerator(BasicMLDataSet owner)
            {
                this.current = -1;
                this.owner = owner;
            }

            /// <summary>
            /// The current data item.
            /// </summary>
            public MLDataPair Current
            {
                get
                {
                    return owner.data[this.current];
                }
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
            object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (this.current < 0)
                    {
                        throw new InvalidOperationException("Must call MoveNext before reading Current.");
                    }
                    return this.owner.data[this.current];
                }
            }

            /// <summary>
            /// Move to the next item.
            /// </summary>
            /// <returns>True if there is a next item.</returns>
            public bool MoveNext()
            {
                this.current++;
                if (current >= this.owner.data.Count)
                    return false;
                return true;
            }

            /// <summary>
            /// Reset to the beginning.
            /// </summary>
            public void Reset()
            {
                this.current = -1;
            }
        }

        /// <summary>
        /// Access to the list of data items.
        /// </summary>
        public virtual IList<MLDataPair> Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
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
        /// Construct a data set from an input and idea array.
        /// </summary>
        /// <param name="input">The input into the neural network for training.</param>
        /// <param name="ideal">The idea into the neural network for training.</param>
        public BasicMLDataSet(double[][] input, double[][] ideal)
        {
            for (int i = 0; i < input.Length; i++)
            {
                double[] tempInput = new double[input[0].Length];
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

                BasicMLData inputData = new BasicMLData(tempInput);
                
                this.Add(inputData, idealData);
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
                if (this.data == null || this.data.Count == 0)
                    return 0;
                MLDataPair pair = this.data[0];
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
                if (this.data == null || this.data.Count == 0)
                    return 0;
                MLDataPair pair = this.data[0];
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
            this.data.Add(pair);
        }

        /// <summary>
        /// Add supervised data to the set.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <param name="idealData">The ideal data.</param>
        public virtual void Add(MLData inputData, MLData idealData)
        {
            MLDataPair pair = new BasicMLDataPair(inputData, idealData);
            this.data.Add(pair);
        }

        /// <summary>
        /// Add a pair to the set.
        /// </summary>
        /// <param name="inputData">The pair to add to the set.</param>
        public virtual void Add(MLDataPair inputData)
        {
            this.data.Add(inputData);
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
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
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
                if (this.data.Count == 0)
                    return false;
                return (this.data[0].Supervised);
            }
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public override object Clone()
        {
            BasicMLDataSet result = new BasicMLDataSet();
            foreach(MLDataPair pair in this.Data )
            {
                result.Add((MLDataPair)pair.Clone());
            }
            return result;
        }

        /// <summary>
        /// Create a persistor to load/save this object to XML.
        /// </summary>
        /// <returns>The persistor.</returns>
        public override IPersistor CreatePersistor()
        {
            return new BasicNeuralDataSetPersistor();
        }

        /// <summary>
        /// The number of records in this data set.
        /// </summary>
        public long Count
        {
            get 
            {
                return this.data.Count;
            }
        }

        /// <summary>
        /// Get one record from the data set.
        /// </summary>
        /// <param name="index">The index to read.</param>
        /// <param name="pair">The pair to read into.</param>
        public void GetRecord(long index, MLDataPair pair)
        {
            MLDataPair source = this.data[(int)index];
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
            return new BasicMLDataSet(this.Data);
        }


        /// <summary>
        /// Return true if supervised.
        /// </summary>
        public bool Supervised
        {
            get 
            {
                if (this.data.Count == 0)
                {
                    return false;
                }
                else
                {
                    return this.data[0].Supervised;
                }
            }
        }
    }
}
