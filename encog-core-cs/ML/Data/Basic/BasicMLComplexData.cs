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
using System.Linq;
using System.Text;
using Encog.MathUtil;
using Encog.Util.KMeans;

namespace Encog.ML.Data.Basic
{
    /// <summary>
    /// This class implements a data object that can hold complex numbers.  It 
    /// implements the interface MLData, so it can be used with nearly any Encog 
    /// machine learning method.  However, not all Encog machine learning methods 
    /// are designed to work with complex numbers.  A Encog machine learning method 
    /// that does not support complex numbers will only be dealing with the 
    /// real-number portion of the complex number. 
    /// </summary>
    [Serializable]
    public class BasicMLComplexData : IMLComplexData
    {
        /// <summary>
        /// The data held by this object.
        /// </summary>
        private ComplexNumber[] _data;

        /// <summary>
        /// Construct this object with the specified data.  Use only real numbers. 
        /// </summary>
        /// <param name="d">The data to construct this object with.</param>
        public BasicMLComplexData(double[] d)
            : this(d.Length)
        {
        }

        /// <summary>
        /// Construct this object with the specified data. Use complex numbers. 
        /// </summary>
        /// <param name="d">The data to construct this object with.</param>
        public BasicMLComplexData(ComplexNumber[] d)
        {
            _data = d;
        }

        /// <summary>
        /// Construct this object with blank data and a specified size. 
        /// </summary>
        /// <param name="size">The amount of data to store.</param>
        public BasicMLComplexData(int size)
        {
            _data = new ComplexNumber[size];
        }

        /// <summary>
        /// Construct a new BasicMLData object from an existing one. This makes a
        /// copy of an array. If MLData is not complex, then only reals will be 
        /// created. 
        /// </summary>
        /// <param name="d">The object to be copied.</param>
        public BasicMLComplexData(IMLData d)
        {
            if (d is IMLComplexData)
            {
                var c = (IMLComplexData) d;
                for (int i = 0; i < d.Count; i++)
                {
                    _data[i] = new ComplexNumber(c.GetComplexData(i));
                }
            }
            else
            {
                for (int i = 0; i < d.Count; i++)
                {
                    _data[i] = new ComplexNumber(d[i], 0);
                }
            }
        }
        
        #region IMLComplexData Members

        /// <summary>
        /// Clear all values to zero.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = new ComplexNumber(0, 0);
            }
        }

        /// <inheritdoc/>
        public Object Clone()
        {
            return new BasicMLComplexData(this);
        }


        /// <summary>
        /// The complex numbers.
        /// </summary>
        public ComplexNumber[] ComplexData
        {
            get { return _data; }
            set { _data = value; }
        }


        /// <inheritdoc/>
        public ComplexNumber GetComplexData(int index)
        {
            return _data[index];
        }
        
        /// <summary>
        /// Set a data element to a complex number. 
        /// </summary>
        /// <param name="index">The index to set.</param>
        /// <param name="d">The complex number.</param>
        public void SetComplexData(int index, ComplexNumber d)
        {
            _data[index] = d;
        }

        /// <summary>
        /// Set the complex data array.
        /// </summary>
        /// <param name="d">A new complex data array.</param>
        public void SetComplexData(ComplexNumber[] d)
        {
            _data = d;
        }

        /// <summary>
        /// Access the data by index.
        /// </summary>
        /// <param name="x">The index to access.</param>
        /// <returns></returns>
        public virtual double this[int x]
        {
            get { return _data[x].Real; }
            set { _data[x] = new ComplexNumber(value, 0); }
        }

        /// <summary>
        /// Get the data as an array.
        /// </summary>
        public virtual double[] Data
        {
            get
            {
                var d = new double[_data.Length];
                for (int i = 0; i < d.Length; i++)
                {
                    d[i] = _data[i].Real;
                }
                return d;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    _data[i] = new ComplexNumber(value[i], 0);
                }
            }
        }

        /// <inheritdoc/>
        public int Count
        {
            get { return _data.Count(); }
        }

        #endregion

        /// <inheritdoc/>
        public override String ToString()
        {
            var builder = new StringBuilder("[");
            builder.Append(GetType().Name);
            builder.Append(":");
            for (int i = 0; i < _data.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }
                builder.Append(_data[i].ToString());
            }
            builder.Append("]");
            return builder.ToString();
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
				target[i + targetIndex] = _data[i].Real;
		}
	}
}
