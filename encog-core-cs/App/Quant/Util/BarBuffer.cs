using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Util;

namespace Encog.App.Quant.Util
{
    /// <summary>
    /// A buffer of bar segments.
    /// </summary>
    public class BarBuffer
    {
        /// <summary>
        /// The bar data loaded.
        /// </summary>
        private IList<double[]> data = new List<double[]>();

        /// <summary>
        /// The number of periods.
        /// </summary>
        private int periods;

        /// <summary>
        /// Determine if the buffer is full.
        /// </summary>
        public bool Full
        {
            get
            {
                return data.Count >= this.periods;
            }
        }

        /// <summary>
        /// The data.
        /// </summary>
        public IList<double[]> Data
        {
            get
            {
                return data;
            }
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        /// <param name="periods"></param>
        public BarBuffer(int periods)
        {
            this.periods = periods;
        }

        /// <summary>
        /// Add to the bars.
        /// </summary>
        /// <param name="d"></param>
        public void Add(double d)
        {
            Add(new double[1] { d });
        }

        /// <summary>
        /// Add to the bars.
        /// </summary>
        /// <param name="d">The data to add.</param>
        public void Add(double[] d)
        {
            data.Insert(0, EngineArray.ArrayCopy(d));
            if (data.Count > periods)
                data.RemoveAt(data.Count - 1);
        }

        /// <summary>
        /// Average all of the bars.
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

            return total / data.Count;
        }

        /// <summary>
        /// Get the average gain.
        /// </summary>
        /// <param name="idx">The field to get the average gain for.</param>
        /// <returns>The average gain.</returns>
        public double AverageGain(int idx)
        {            
            double total = 0;
            int count = 0;
            for (int i = 0; i < data.Count-1; i++)
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
                return 0;
            else
                return total / count;
        }

        /// <summary>
        /// Get the average loss.
        /// </summary>
        /// <param name="idx">The index to check for.</param>
        /// <returns>The average loss.</returns>
        public double AverageLoss(int idx)
        {
            double total = 0;
            int count = 0;
            for (int i = 0; i < data.Count-1; i++)
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
                return 0;
            else
                return total / count;
        }

        /// <summary>
        /// get the max for the specified index.
        /// </summary>
        /// <param name="idx">The index to check.</param>
        /// <returns>The max.</returns>
        public double Max(int idx)
        {
            double result = Double.MinValue;

            foreach (double[] d in this.data)
            {
                result = Math.Max(d[idx], result);
            }
            return result;
        }

        /// <summary>
        /// get the min for the specified index.
        /// </summary>
        /// <param name="idx">The index to check.</param>
        /// <returns>The min.</returns>
        public double Min(int idx)
        {
            double result = Double.MaxValue;

            foreach (double[] d in this.data)
            {
                result = Math.Min(d[idx], result);
            }
            return result;
        }

        /// <summary>
        /// Pop (and remove) the oldest bar in the buffer.
        /// </summary>
        /// <returns>The oldest bar in the buffer.</returns>
        public double[] Pop()
        {
            if (this.data.Count == 0)
                return null;

            int idx = this.data.Count-1;
            double[] result = this.data[idx];
            this.data.RemoveAt(idx);
            return result;
        }
    }
}
