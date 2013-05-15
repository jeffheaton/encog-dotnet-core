//
// Encog(tm) Core v3.2 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
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
            for (int i = 0; i < value.Count; i++)
                value.Data[i] = ((value.Data[i] * value.Count + d[i]) / (value.Count + 1));
        }

        /// <inheritdoc/>
        public void Remove(IMLData d)
        {
            for (int i = 0; i < value.Count; i++)
                value[i] =
                    ((value[i] * value.Count - d[i]) / (value.Count - 1));
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
