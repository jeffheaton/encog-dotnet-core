using System;
using Encog.Util.KMeans;

namespace Encog.ML.Data.Basic
{
    /// <summary>
    /// A basic implementation of a centroid.
    /// </summary>
    public class BasicMLDataCentroid : ICentroid<IMLData>
    {
        /// <summary>
        /// The value this centroid is based on.
        /// </summary>
        private BasicMLData value;

        /// <summary>
        /// Construct the centroid. 
        /// </summary>
        /// <param name="o">The object to base the centroid on.</param>
        public BasicMLDataCentroid(IMLData o)
        {
            this.value = (BasicMLData)o.Clone();
        }

        /// <inheritdoc/>
        public void Add(IMLData d)
        {
            double[] a = d.Data;

            for (int i = 0; i < value.Count; i++)
                value.Data[i] =
                    ((value.Data[i] * value.Count + a[i]) / (value.Count + 1));
        }

        /// <inheritdoc/>
        public void Remove(IMLData d)
        {
            double[] a = d.Data;

            for (int i = 0; i < value.Count; i++)
                value[i] =
                    ((value[i] * value.Count - a[i]) / (value.Count - 1));
        }

        /// <inheritdoc/>
        public double Distance(IMLData d)
        {
            IMLData diff = value.Minus(d);
            double sum = 0.0;

            for (int i = 0; i < diff.Count; i++)
                sum += diff[i] * diff[i];

            return Math.Sqrt(sum);
        }
    }
}
