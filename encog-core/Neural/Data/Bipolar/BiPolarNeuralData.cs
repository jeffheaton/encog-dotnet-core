using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Matrix;

namespace Encog.Neural.Data.Bipolar
{
    class BiPolarNeuralData : INeuralData
    {
        /// <summary>
        /// The data held by this object.
        /// </summary>
        private bool[] data;

        /// <summary>
        /// Construct this object with the specified data. 
        /// </summary>
        /// <param name="d">The data to create this object with.</param>
        public BiPolarNeuralData(bool[] d)
        {
            this.data = new bool[d.Length];
            for (int i = 0; i < d.Length; i++)
            {
                this.data[i] = d[i];
            }
        }

        /// <summary>
        /// Construct a data object with the specified size.
        /// </summary>
        /// <param name="size">The size of this data object.</param>
        public BiPolarNeuralData(int size)
        {
            this.data = new bool[size];
        }

        public double this[int x]
        {
            get
            {
                return BiPolarUtil.Bipolar2double(this.data[x]);
            }
            set
            {
                this.data[x] = BiPolarUtil.Double2bipolar(value);
            }
        }

        public double[] Data
        {
            get
            {
                return BiPolarUtil.Bipolar2double(this.data);
            }
            set
            {
                this.data = BiPolarUtil.Double2bipolar(value);
            }
        }

        public int Count
        {
            get
            {
                return this.data.Length;
            }
        }

        /// <summary>
        /// Get the specified data item as a boolean.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool GetBoolean(int i)
        {
            return this.data[i];
        }
    }
}
