//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.ML;
using Encog.ML.Data;
using Encog.Util.Error;

namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// Calculate a score based on a training set. This class allows simulated
    /// annealing or genetic algorithms just as you would any other training set
    /// based training method.
    /// </summary>
    ///
    public class TrainingSetScore : ICalculateScore
    {
        /// <summary>
        /// The training set.
        /// </summary>
        ///
        private readonly IMLDataSet training;

        /// <summary>
        /// Construct a training set score calculation.
        /// </summary>
        ///
        /// <param name="training_0">The training data to use.</param>
        public TrainingSetScore(IMLDataSet training_0)
        {
            training = training_0;
        }

        #region ICalculateScore Members

        /// <summary>
        /// Calculate the score for the network.
        /// </summary>
        ///
        /// <param name="method">The network to calculate for.</param>
        /// <returns>The score.</returns>
        public double CalculateScore(IMLRegression method)
        {
            return CalculateRegressionError.CalculateError(method, training);
        }

        /// <summary>
        /// A training set based score should always seek to lower the error,
        /// as a result, this method always returns true.
        /// </summary>
        ///
        /// <returns>Returns true.</returns>
        public bool ShouldMinimize
        {
            get { return true; }
        }

        #endregion
    }
}
