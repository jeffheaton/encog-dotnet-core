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
    /// End training once the error falls below a specified level.
    /// </summary>
    public class EndMaxErrorStrategy : IEndTrainingStrategy
    {
        /// <summary>
        /// The max error.
        /// </summary>
        private readonly double _maxError;

        /// <summary>
        /// Has training started.
        /// </summary>
        private bool _started;

        /// <summary>
        /// The trainer.
        /// </summary>
        private IMLTrain _train;

        /// <summary>
        /// Construct the object, specify the max error.
        /// </summary>
        /// <param name="maxError">The max error.</param>
        public EndMaxErrorStrategy(double maxError)
        {
            _maxError = maxError;
            _started = false;
        }

        #region EndTrainingStrategy Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual bool ShouldStop()
        {
            return _started && _train.Error < _maxError;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void Init(IMLTrain train_0)
        {
            _train = train_0;
            _started = false;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PostIteration()
        {
            _started = true;
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
