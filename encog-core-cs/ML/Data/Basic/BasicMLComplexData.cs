using System;
using System.Linq;
using System.Text;
using Encog.MathUtil;

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
        public String ToString()
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
    }
}