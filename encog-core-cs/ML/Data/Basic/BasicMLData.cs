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

namespace Encog.ML.Data.Basic
{
    /// <summary>
    /// Basic implementation of the NeuralData interface that stores the
    /// data in an array.  
    /// </summary>
    [Serializable]
	public class BasicMLData: IMLDataModifiable
    {
        protected double[] _data;

        /// <summary>
        /// Construct this object with the specified data. 
        /// </summary>
        /// <param name="d">The data to construct this object with.</param>
        public BasicMLData(double[] d, bool copy = true)
        {
			if(copy)
			{
				_data = new double[d.Length];
				EngineArray.ArrayCopy(d, _data);
			}
			else _data = d;
        }


        /// <summary>
        /// Construct this object with blank data and a specified size.
        /// </summary>
        /// <param name="size">The amount of data to store.</param>
        public BasicMLData(int size)
        {
            _data = new double[size];
        }

        /// <summary>
        /// Access the data by index.
        /// </summary>
        /// <param name="x">The index to access.</param>
        /// <returns></returns>
        public virtual double this[int x]
        {
            get { return _data[x]; }
            set { _data[x] = value; }
        }

        /// <summary>
        /// Get the data as an array.
        /// </summary>
        public virtual double[] Data
        {
            get { return _data; }
        }

        /// <summary>
        /// Get the count of data items.
        /// </summary>
        public virtual int Count
        {
            get { return _data.Length; }
        }

        /// <summary>
        /// Convert the object to a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append('[');
            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                    result.Append(',');
                result.Append(Data[i]);
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
            var result = new BasicMLData(_data);
            return result;
        }

        /// <summary>
        /// Clear to zero.
        /// </summary>
        public void Clear()
        {
            EngineArray.Fill(_data, 0);
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

            var result = new BasicMLData(Count);
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
            var result = new BasicMLData(Count);

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

            var result = new BasicMLData(Count);
            for (int i = 0; i < Count; i++)
                result[i] = this[i] - o[i];

            return result;
        }

		public void CopyTo(double[] target, int targetIndex, int count)
		{
			EngineArray.ArrayCopy(_data, 0, target, targetIndex, count);
		}
	}
}
