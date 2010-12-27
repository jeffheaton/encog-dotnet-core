using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Util;

namespace Encog.App.Quant.Util
{
    public class BarBuffer
    {
        private IList<double[]> data = new List<double[]>();
        private int periods;

        public bool Full
        {
            get
            {
                return data.Count >= this.periods;
            }
        }

        public IList<double[]> Data
        {
            get
            {
                return data;
            }
        }

        public BarBuffer(int periods)
        {
            this.periods = periods;
        }

        public void Add(double d)
        {
            Add(new double[1] { d });
        }

        public void Add(double[] d)
        {
            data.Insert(0, EngineArray.ArrayCopy(d));
            if (data.Count > periods)
                data.RemoveAt(data.Count - 1);
        }

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

        public double Max(int idx)
        {
            double result = Double.MinValue;

            foreach (double[] d in this.data)
            {
                result = Math.Max(d[idx], result);
            }
            return result;
        }

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
