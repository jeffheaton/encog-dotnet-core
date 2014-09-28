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
namespace Encog.Neural.Prune
{
    /// <summary>
    /// Specifies the minimum and maximum neuron counts for a layer.
    /// </summary>
    ///
    public class HiddenLayerParams
    {
        /// <summary>
        /// The maximum number of neurons on this layer.
        /// </summary>
        ///
        private readonly int _max;

        /// <summary>
        /// The minimum number of neurons on this layer.
        /// </summary>
        ///
        private readonly int _min;

        /// <summary>
        /// Construct a hidden layer param object with the specified min and max
        /// values.
        /// </summary>
        ///
        /// <param name="min">The minimum number of neurons.</param>
        /// <param name="max">The maximum number of neurons.</param>
        public HiddenLayerParams(int min, int max)
        {
            _min = min;
            _max = max;
        }


        /// <value>The maximum number of neurons.</value>
        public int Max
        {
            get { return _max; }
        }


        /// <value>The minimum number of neurons.</value>
        public int Min
        {
            get { return _min; }
        }
    }
}
