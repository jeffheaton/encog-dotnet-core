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
using System.Text;
using Encog.Util;
using Encog.Util.KMeans;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Encog.ML.Data.Basic;

namespace Encog.ML.Data.Sparse
{
    /// <summary>
    /// Basic implementation of the NeuralData interface that stores the
    /// data in an sparse vector.
    /// </summary>
    [Serializable]
    public class SparseMLData : IMLDataModifiable, IEnumerable<double>, IHasArithimeticOperations
    {
        /// <summary>
        /// Stores the data, but looked up in a dictionary instead of an array.
        /// For arrays where most of the values are default, this will save RAM.
        /// </summary>
        protected Dictionary<int, double> SparseData = new Dictionary<int, double>();

        /// <inheritdoc/>
        public int Count { get; set; }

        /// <summary>
        /// The default value of the sparse vector (defaults to 1.0d). Data which has this
        /// value is not stored, so the correct choice is whichever value is most common
        /// in the data set.
        /// </summary>
        protected double DefaultValue { get; set; }

        /// <summary>
        /// A measure of how sparse the vector is, lower values mean that it is sparser.
        /// For example, if there are 4 values in the vector, but 2 are the default value,
        /// 0.5 will be returned.  If there are 4 values in the vector, but 3 are the default
        /// value, 0.25 will be returned.
        /// </summary>
        public double FillPercentage
        {
            get
            {
                return this.Count == 0 ? 0d : this.SparseData.Count / (double)this.Count;
            }
        }

        /// <summary>
        /// Creates an instance of the SparseML data type, initializing the underling data
        /// and optionally specifiying the default value for data. Data which has the default value is not stored.
        /// </summary>
        public SparseMLData(IEnumerable<double> data, double defaultValue = 0d)
            : this(defaultValue)
        {
            this.DefaultValue = defaultValue;
            this.AddRange(data);
        }

        /// <summary>
        /// Creates an instance of the SparseML data type, specifiying the default value for 
        /// data. Data which has the default value is not stored.
        /// </summary>
        public SparseMLData(double defaultValue)
            : this()
        {
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Creates a default instance of the SparseML data type.
        /// </summary>
        public SparseMLData()
        {
        }

        /// <summary>
        /// Construct this object with blank data and a specified size.
        /// </summary>
        /// <param name="size">The amount of data to store.</param>
        public SparseMLData(int size, double defaultValue = 0d)
            : this(defaultValue)
        {
            this.Count = size;
        }

        /// <summary>
        /// Add data to the underlying dictionary.
        /// </summary>
        /// <param name="items"></param>
        public void Add(double item)
        {
            if (item != this.DefaultValue)
            {
                this.SparseData.Add(this.Count, item);
            }
            this.Count++;
        }

        /// <summary>
        /// Add data to the underlying dictionary.
        /// </summary>
        /// <param name="items"></param>
        public void Add(params double[] items)
        {
            AddRange(items);
        }

        /// <summary>
        /// Add data to the underlying dictionary.
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<double> items)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Access the data by index.
        /// </summary>
        /// <param name="index">The index to access.</param>
        /// <returns></returns>
        public double this[int index]
        {
            get
            {
                if (this.SparseData.ContainsKey(index))
                {
                    return this.SparseData[index];
                }
                else
                {
                    return this.DefaultValue;
                }
            }
            set
            {
                if (this.SparseData.ContainsKey(index))
                {
                    this.SparseData[index] = value;
                }
                else
                {
                    this.SparseData.Add(index, value);
                }
            }
        }

        /// <summary>
        /// Enumerates through the values.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<double> GetEnumerator()
        {
            return this.Enumerate().GetEnumerator();
        }

        /// <summary>
        /// Enumerates through the values.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<double> Enumerate()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Get the data as an array.
        /// </summary>
        public virtual double[] Data
        {
            get { return this.ToArray(); }
        }

        /// <summary>
        /// Convert the object to a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override string ToString()
        {
            var result = new StringBuilder("[");
            foreach (var iv in this.Select((value, index) => new { value, index }))
            {
                if (iv.index > 0)
                {
                    result.Append(',');
                }
                result.Append(iv.value);
            }
            result.Append(']');
            return result.ToString();
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public object Clone()
        {
            var result = new SparseMLData(this, this.DefaultValue);
            return result;
        }

        /// <summary>
        /// Clear to zero.
        /// </summary>
        public void Clear()
        {
            this.SparseData.Clear();
            this.Count = 0;
        }

        /// <inheritdoc/>
        public ICentroid<IMLData> CreateCentroid()
        {
            return new BasicMLDataCentroid(this);
        }

        /// <summary>
        /// Add one data element to another.  This does not modify the object.
        /// </summary>
        /// <param name="o">The other data element</param>
        /// <returns>The result.</returns>
        public IMLData Plus(IMLData o)
        {
            if (Count != o.Count)
                throw new EncogError("Lengths must match.");

            var result = new SparseMLData(this.Count, this.DefaultValue);
            for (int i = 0; i < Count; i++)
                result[i] = this[i] + o[i];

            return result;
        }

        /// <summary>
        /// Multiply one data element with another.  This does not modify the object.
        /// </summary>
        /// <param name="d">The other data element</param>
        /// <returns>The result.</returns>
        public IMLData Times(double d)
        {
            var result = new SparseMLData(this.Count, this.DefaultValue);

            for (int i = 0; i < Count; i++)
                result[i] = this[i] * d;

            return result;
        }

        /// <summary>
        /// Subtract one data element from another.  This does not modify the object.
        /// </summary>
        /// <param name="o">The other data element</param>
        /// <returns>The result.</returns>
        public IMLData Minus(IMLData o)
        {
            if (Count != o.Count)
                throw new EncogError("Counts must match.");

            var result = new SparseMLData(this.Count, this.DefaultValue);
            for (int i = 0; i < Count; i++)
                result[i] = this[i] - o[i];

            return result;
        }

        /// <summary>
        /// Copies the source data to the target array at the specified index.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetIndex"></param>
        /// <param name="count">The maximum number of items to copy.</param>
        public void CopyTo(double[] target, int targetIndex, int count)
        {
            int position = targetIndex;
            int copyCount = 0;
            foreach (var d in this)
            {
                target[position] = d;
                position++;
                copyCount++;

                if (copyCount == count)
                {
                    break;
                }
            }
        }
    }
}
