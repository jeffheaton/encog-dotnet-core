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
        /// How many items have been added to the centroid.
        /// </summary>
        private int _size;

        /// <summary>
        /// Construct the centroid. 
        /// </summary>
        /// <param name="o"> The pair to base the centroid on.</param>
        public BasicMLDataPairCentroid(BasicMLDataPair o)
        {
            _value = (BasicMLData)o.Input.Clone();
            _size = 1;
        }

        /// <inheritdoc/>
        public void Remove(IMLDataPair d)
        {
            for (int i = 0; i < _value.Count; i++)
                _value[i] = ((_value[i] * _size - d.Input[i]) / (_size - 1));
            _size--;
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
            for (int i = 0; i < _value.Count; i++)
                _value[i] =
                    ((_value[i] * _size) + d.Input[i]) / (_size + 1);
            _size++;
        }

    }
}
