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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Arrayutil
{
    /// <summary>
    /// This class implements a simple sliding window. Arrays of doubles can be added
    /// to the window. The sliding window will fill up to the specified size.
    /// Additional entries will cause the oldest entries to fall off.
    /// </summary>
    public class WindowDouble
    {
        /// <summary>
        /// The size of the window.
        /// 
        /// </summary>
        private int size;

        /// <summary>
        /// The data in the window.
        /// </summary>
        private IList<double[]> data = new List<double[]>();

        /// <summary>
        /// Construct the window. 
        /// </summary>
        /// <param name="theSize">The size of the window.</param>
        public WindowDouble(int theSize)
        {
            this.size = theSize;
        }

        /// <summary>
        /// Add an array to the window. 
        /// </summary>
        /// <param name="a">The array.</param>
        public void Add(double[] a)
        {
            this.data.Insert(0, a);
            while (this.data.Count > this.size)
            {
                this.data.RemoveAt(this.data.Count - 1);
            }
        }

        /// <summary>
        /// Clear the contents of the window.
        /// </summary>
        public void Clear()
        {
            this.data.Clear();
        }

        /// <summary>
        /// True, if the window is full.
        /// </summary>
        /// <returns>True, if the window is full.</returns>
        public bool IsFull()
        {
            return this.data.Count == this.size;
        }

        /// <summary>
        /// Calculate the max value, for the specified index, over all of the data in
        /// the window. 
        /// </summary>
        /// <param name="index">The index of the value to compare.</param>
        /// <param name="starting">The starting position, inside the window to compare at.</param>
        /// <returns>THe max value.</returns>
        public double CalculateMax(int index, int starting)
        {
            double result = double.NegativeInfinity;

            for (int i = starting; i < this.data.Count; i++)
            {
                double[] a = this.data[i];
                result = Math.Max(a[index], result);
            }

            return result;
        }
        
        /// <summary>
        /// Calculate the max value, for the specified index, over all of the data in
        /// the window. 
        /// </summary>
        /// <param name="index">The index of the value to compare.</param>
        /// <param name="starting">The starting position, inside the window to compare at.</param>
        /// <returns>THe max value.</returns>
        public double CalculateMin(int index, int starting)
        {
            double result = double.PositiveInfinity;

            for (int i = starting; i < this.data.Count; i++)
            {
                double[] a = this.data[i];
                result = Math.Min(a[index], result);
            }

            return result;
        }

        /// <summary>
        /// Get the last value from the window. This is the most recent item added. 
        /// </summary>
        /// <returns>The last value from the window.</returns>
        public double[] GetLast()
        {
            return this.data[0];
        }
    }
}
