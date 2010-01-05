// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Matrix;
using Encog.Neural.Data;

namespace Encog.Neural.NeuralData.Bipolar
{
    /// <summary>
    /// A NeuralData implementation designed to work with bipolar data.
    /// Bipolar data contains two values.  True is stored as 1, and false
    /// is stored as -1.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class BiPolarNeuralData : INeuralData
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

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        /// <param name="x">The index.</param>
        /// <returns>The value at the specified index.</returns>
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

        /// <summary>
        /// Get the data as an array.
        /// </summary>
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

        /// <summary>
        /// The size of the array.
        /// </summary>
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

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public Object Clone()
        {
            return new BiPolarNeuralData(this.data);
        }

        /// <summary>
        /// Set the value as a boolean.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="b">The boolean value.</param>
        public void SetBoolean(int index, bool b)
        {
            this.data[index] = b;
        }
    }
}
