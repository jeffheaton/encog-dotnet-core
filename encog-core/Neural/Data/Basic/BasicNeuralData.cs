using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Persist;

namespace Encog.Neural.Data.Basic
{
    class BasicNeuralData : INeuralData
    {
        private double[] data;

        /// <summary>
        /// Construct this object with the specified data. 
        /// </summary>
        /// <param name="d">The data to construct this object with.</param>
        public BasicNeuralData(double[] d)
            : this(d.Length)
        {
            for (int i = 0; i < d.Length; i++)
            {
                this.data[i] = d[i];
            }
        }


        /// <summary>
        /// Construct this object with blank data and a specified size.
        /// </summary>
        /// <param name="size">The amount of data to store.</param>
        public BasicNeuralData(int size)
        {
            this.data = new double[size];
        }

        public double this[int x]
        {
            get
            {
                return this.data[x];
            }
            set
            {
                this.data[x] = value;
            }
        }

        public double[] Data
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

        public int Count
        {
            get
            {
                return this.data.Length;
            }
        }
    }
}
