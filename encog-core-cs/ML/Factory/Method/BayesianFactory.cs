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
using Encog.ML.Bayesian;

namespace Encog.ML.Factory.Method
{
    /// <summary>
    /// Factory to create Bayesian networks
    /// </summary>
    public class BayesianFactory
    {
        /// <summary>
        /// Create a bayesian network.
        /// </summary>
        /// <param name="architecture">The architecture to use.</param>
        /// <param name="input">The input neuron count.</param>
        /// <param name="output">The output neuron count.</param>
        /// <returns>The new bayesian network.</returns>
        public IMLMethod Create(String architecture, int input,
                                int output)
        {
            var method = new BayesianNetwork {Contents = architecture};
            return method;
        }
    }
}
