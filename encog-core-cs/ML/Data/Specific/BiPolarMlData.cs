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
using Encog.MathUtil.Matrices;
using System.Text;
using Encog.Util.KMeans;

namespace Encog.ML.Data.Specific
{
    /// <summary>
    /// A NeuralData implementation designed to work with bipolar data.
    /// Bipolar data contains two values.  True is stored as 1, and false
    /// is stored as -1.
    /// </summary>
    [Serializable]
	public class BiPolarMLData: IMLDataModifiable
    {
        /// <summary>
        /// The data held by this object.
        /// </summary>
        private bool[] _data;

        /// <summary>
        /// Construct this object with the specified data. 
        /// </summary>
        /// <param name="d">The data to create this object with.</param>
        public BiPolarMLData(bool[] d) : this(d.Length)
        {
            for (int i = 0; i < d.Length; i++)
            {
                _data[i] = d[i];
            }
        }

        /// <summary>
        /// Construct a data object with the specified size.
        /// </summary>
        /// <param name="size">The size of this data object.</param>
        public BiPolarMLData(int size)
        {
            _data = new bool[size];
        }

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        /// <param name="x">The index.</param>
        /// <returns>The value at the specified index.</returns>
        public double this[int x]
        {
            get { return BiPolarUtil.Bipolar2double(_data[x]); }
            set { _data[x] = BiPolarUtil.Double2bipolar(value); }
        }

        /// <summary>
        /// Get the data as an array.
        /// </summary>
        public double[] Data
        {
            get { return BiPolarUtil.Bipolar2double(_data); }
			internal set { _data = BiPolarUtil.Double2bipolar(value); }
        }

        /// <summary>
        /// The size of the array.
        /// </summary>
        public int Count
        {
            get { return _data.Length; }
        }

        /// <summary>
        /// Get the specified data item as a boolean.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool GetBoolean(int i)
        {
            return _data[i];
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public virtual object Clone()
        {
            return new BiPolarMLData(_data);
        }

        /// <summary>
        /// Set the value as a boolean.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="b">The boolean value.</param>
        public void SetBoolean(int index, bool b)
        {
            _data[index] = b;
        }

        /// <summary>
        /// Clear to false.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = false;
            }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append('[');
            for (var i = 0; i < Count; i++)
            {
                result.Append(this[i] > 0 ? "T" : "F");
                if (i != Count - 1)
                {
                    result.Append(",");
                }
            }
            result.Append(']');
            return (result.ToString());
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns>Nothing.</returns>
        public ICentroid<IMLData> CreateCentroid()
        {
            return null;
        }

		public void CopyTo(double[] target, int targetIndex, int count)
		{
			for(int i = 0; i < count; i++)
				target[i + targetIndex] = _data[i] ? 1.0 : -1.0;
		}
	}
}
