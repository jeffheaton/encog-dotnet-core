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
namespace Encog.ML.Train.Strategy.End
{
    /// <summary>
    /// End the training when a specified number of iterations has been reached.
    /// </summary>
    public class EndIterationsStrategy : IEndTrainingStrategy
    {
        private readonly int _maxIterations;
        private int _currentIteration;
        private IMLTrain _train;

        /// <summary>
        /// Construct the object, specify the max number of iterations.
        /// </summary>
        /// <param name="maxIterations">The number of iterations.</param>
        public EndIterationsStrategy(int maxIterations)
        {
            _maxIterations = maxIterations;
            _currentIteration = 0;
        }

        #region EndTrainingStrategy Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual bool ShouldStop()
        {
            return (_currentIteration >= _maxIterations);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void Init(IMLTrain train_0)
        {
            _train = train_0;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PostIteration()
        {
            _currentIteration = _train.IterationNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PreIteration()
        {
        }

        #endregion
    }
}
