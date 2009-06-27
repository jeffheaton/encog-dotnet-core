// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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
using Encog.Neural.NeuralData;
using System.Runtime.Serialization;

namespace Encog.Neural.Data.Basic
{
    /// <summary>
    /// Basic implementation of the NeuralData interface that stores the
    /// data in an array.  
    /// </summary>
    [Serializable]
    public class BasicNeuralData : INeuralData
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

        /// <summary>
        /// Access the data by index.
        /// </summary>
        /// <param name="x">The index to access.</param>
        /// <returns></returns>
        public virtual double this[int x]
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

        /// <summary>
        /// Get the data as an array.
        /// </summary>
        public virtual double[] Data
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
        /// Get the count of data items.
        /// </summary>
        public virtual int Count
        {
            get
            {
                return this.data.Length;
            }
        }

        /// <summary>
        /// Convert the object to a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append('[');
            for (int i = 0; i < this.Count; i++)
            {
                if (i > 0)
                    result.Append(',');
                result.Append(this.Data[i]);
            }
            result.Append(']');
            return result.ToString();
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public Object Clone()
        {
            BasicNeuralData result = new BasicNeuralData(this.data);
            return result;
        }
    }
}
