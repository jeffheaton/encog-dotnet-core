//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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

namespace Encog.ML.Data.Basic
{
    /// <summary>
    /// Basic implementation of the NeuralData interface that stores the
    /// data in an array.  
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class BasicMLData : MLData
    {
        private double[] data;

        /// <summary>
        /// Construct this object with the specified data. 
        /// </summary>
        /// <param name="d">The data to construct this object with.</param>
        public BasicMLData(double[] d)
            : this(d.Length)
        {
            for (int i = 0; i < d.Length; i++)
            {
                data[i] = d[i];
            }
        }


        /// <summary>
        /// Construct this object with blank data and a specified size.
        /// </summary>
        /// <param name="size">The amount of data to store.</param>
        public BasicMLData(int size)
        {
            data = new double[size];
        }

        /// <summary>
        /// Access the data by index.
        /// </summary>
        /// <param name="x">The index to access.</param>
        /// <returns></returns>
        public virtual double this[int x]
        {
            get { return data[x]; }
            set { data[x] = value; }
        }

        /// <summary>
        /// Get the data as an array.
        /// </summary>
        public virtual double[] Data
        {
            get { return data; }
            set { data = value; }
        }

        /// <summary>
        /// Get the count of data items.
        /// </summary>
        public virtual int Count
        {
            get { return data.Length; }
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
            var result = new BasicMLData(data);
            return result;
        }

        /// <summary>
        /// Clear to zero.
        /// </summary>
        public void Clear()
        {
            EngineArray.Fill(data, 0);
        }
    }
}
