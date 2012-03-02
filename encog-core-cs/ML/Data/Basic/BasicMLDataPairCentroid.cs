using System;
using Encog.Util.KMeans;

namespace Encog.ML.Data.Basic
{
    /// <summary>
    /// A centroid for BasicMLDataPair
    /// </summary>
    public class BasicMLDataPairCentroid : ICentroid<IMLDataPair>
    {
        /// <summary>
        /// The value the centroid is based on.
        /// </summary>
        private readonly BasicMLData _value;

        /// <summary>
        /// Construct the centroid. 
        /// </summary>
        /// <param name="o"> The pair to base the centroid on.</param>
        public BasicMLDataPairCentroid(BasicMLDataPair o)
        {
            _value = (BasicMLData)o.Input.Clone();
        }

        /// <inheritdoc/>
        public void Remove(IMLDataPair d)
        {
            double[] a = d.InputArray;

            for (int i = 0; i < _value.Count; i++)
                _value[i] =
                    ((_value[i] * _value.Count - a[i]) / (_value.Count - 1));
        }

        /// <inheritdoc/>
        public double Distance(IMLDataPair d)
        {
            IMLData diff = _value.Minus(d.Input);
            double sum = 0.0;

            for (int i = 0; i < diff.Count; i++)
                sum += diff[i] * diff[i];

            return Math.Sqrt(sum);
        }

        /// <inheritdoc/>
        public void Add(IMLDataPair d)
        {
            double[] a = d.InputArray;

            for (int i = 0; i < _value.Count; i++)
                _value[i] =
                    ((_value[i] * _value.Count) + a[i]) / (_value.Count + 1);
        }

    }
}
