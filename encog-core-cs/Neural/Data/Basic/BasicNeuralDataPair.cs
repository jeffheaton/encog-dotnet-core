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
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Data.Basic
{
    /// <summary>
    /// This is an alias class for Encog 2.5 compatibility.  This class aliases 
    /// BasicMLDataPair.  Newer code should use BasicMLDataPair in place of this class.
    /// </summary>
    public class BasicNeuralDataPair : BasicMLDataPair, INeuralDataPair
    {
        /// <summary>
        /// Construct the pair from input and ideal for supervised.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="ideal">The ideal data.</param>
        public BasicNeuralDataPair(IMLData input, IMLData ideal) : base(input, ideal)
        {
        }

        /// <summary>
        /// Construct the pair from input only for unsupervised.
        /// </summary>
        /// <param name="input">The input data.</param>
        public BasicNeuralDataPair(IMLData input) : base(input)
        {
        }
    }
}
