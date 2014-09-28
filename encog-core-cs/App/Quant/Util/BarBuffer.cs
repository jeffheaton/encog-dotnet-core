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
using Encog.Util;

namespace Encog.App.Quant.Util
{
    /// <summary>
    ///     A buffer of bar segments.
    /// </summary>
    public class BarBuffer
    {
        /// <summary>
        ///     The bar data loaded.
        /// </summary>
        private readonly IList<double[]> data;

        /// <summary>
        ///     The number of periods.
        /// </summary>
        private readonly int periods;

        /// <summary>
        ///     Construct the object.
        /// </summary>
        /// <param name="thePeriods">The number of periods.</param>
        public BarBuffer(int thePeriods)
        {
            data = new List<double[]>();
            periods = thePeriods;
        }

        /// <value>The data.</value>
        public IList<double[]> Data
        {
            get { return data; }
        }


        /// <summary>
        ///     Determine if the buffer is full.
        /// </summary>
        /// <value>True if the buffer is full.</value>
        public bool Full
        {
            get { return data.Count >= periods; }
        }

        /// <summary>
        ///     Add a bar.
        /// </summary>
        /// <param name="d">The bar data.</param>
        public void Add(double d)
        {
            var da = new double[1];
            da[0] = d;
            Add(da);
        }

        /// <summary>
        ///     Add a bar.
        /// </summary>
        /// <param name="d">The bar data.</param>
        public void Add(double[] d)
        {
            data.Insert(0, EngineArray.ArrayCopy(d));
            if (data.Count > periods)
            {
                data.RemoveAt(data.Count - 1);
            }
        }

        /// <summary>
        ///     Average all of the bars.
        /// </summary>
        /// <param name="idx">The bar index to average.</param>
        /// <returns>The average.</returns>
        public double Average(int idx)
        {
            double total = 0;
            for (int i = 0; i < data.Count; i++)
            {
                double[] d = data[i];
                total += d[idx];
            }

            return total/data.Count;
        }

        /// <summary>
        ///     Get the average gain.
        /// </summary>
        /// <param name="idx">The field to get the average gain for.</param>
        /// <returns>The average gain.</returns>
        public double AverageGain(int idx)
        {
            double total = 0;
            int count = 0;
            for (int i = 0; i < data.Count - 1; i++)
            {
                double[] today = data[i];
                double[] yesterday = data[i + 1];
                double diff = today[idx] - yesterday[idx];
                if (diff > 0)
                {
                    total += diff;
                }
                count++;
            }

            if (count == 0)
            {
                return 0;
            }
            else
            {
                return total/count;
            }
        }

        /// <summary>
        ///     Get the average loss.
        /// </summary>
        /// <param name="idx">The index to check for.</param>
        /// <returns>The average loss.</returns>
        public double AverageLoss(int idx)
        {
            double total = 0;
            int count = 0;
            for (int i = 0; i < data.Count - 1; i++)
            {
                double[] today = data[i];
                double[] yesterday = data[i + 1];
                double diff = today[idx] - yesterday[idx];
                if (diff < 0)
                {
                    total += Math.Abs(diff);
                }
                count++;
            }

            if (count == 0)
            {
                return 0;
            }
            else
            {
                return total/count;
            }
        }


        /// <summary>
        ///     Get the max for the specified index.
        /// </summary>
        /// <param name="idx">The index to check.</param>
        /// <returns>The max.</returns>
        public double Max(int idx)
        {
            double result = Double.MinValue;


            foreach (var d  in  data)
            {
                result = Math.Max(d[idx], result);
            }
            return result;
        }

        /// <summary>
        ///     Get the min for the specified index.
        /// </summary>
        /// <param name="idx">The index to check.</param>
        /// <returns>The min.</returns>
        public double Min(int idx)
        {
            double result = Double.MaxValue;


            foreach (var d  in  data)
            {
                result = Math.Min(d[idx], result);
            }
            return result;
        }

        /// <summary>
        ///     Pop (and remove) the oldest bar in the buffer.
        /// </summary>
        /// <returns>The oldest bar in the buffer.</returns>
        public double[] Pop()
        {
            if (data.Count == 0)
            {
                return null;
            }

            int idx = data.Count - 1;
            double[] result = data[idx];
            data.RemoveAt(idx);
            return result;
        }
    }
}
