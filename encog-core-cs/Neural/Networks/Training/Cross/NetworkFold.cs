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
using Encog.Neural.Flat;
using Encog.Util;

namespace Encog.Neural.Networks.Training.Cross
{
    /// <summary>
    /// The network for one fold of a cross validation.
    /// </summary>
    ///
    public class NetworkFold
    {
        /// <summary>
        /// The output for this fold.
        /// </summary>
        ///
        private readonly double[] _output;

        /// <summary>
        /// The weights for this fold.
        /// </summary>
        ///
        private readonly double[] _weights;

        /// <summary>
        /// Construct a fold from the specified flat network.
        /// </summary>
        ///
        /// <param name="flat">THe flat network.</param>
        public NetworkFold(FlatNetwork flat)
        {
            _weights = EngineArray.ArrayCopy(flat.Weights);
            _output = EngineArray.ArrayCopy(flat.LayerOutput);
        }


        /// <value>The network weights.</value>
        public double[] Weights
        {
            get { return _weights; }
        }


        /// <value>The network output.</value>
        public double[] Output
        {
            get { return _output; }
        }

        /// <summary>
        /// Copy weights and output to the network.
        /// </summary>
        ///
        /// <param name="target">The network to copy to.</param>
        public void CopyToNetwork(FlatNetwork target)
        {
            EngineArray.ArrayCopy(_weights, target.Weights);
            EngineArray.ArrayCopy(_output, target.LayerOutput);
        }

        /// <summary>
        /// Copy the weights and output from the network.
        /// </summary>
        ///
        /// <param name="source">The network to copy from.</param>
        public void CopyFromNetwork(FlatNetwork source)
        {
            EngineArray.ArrayCopy(source.Weights, _weights);
            EngineArray.ArrayCopy(source.LayerOutput, _output);
        }
    }
}
