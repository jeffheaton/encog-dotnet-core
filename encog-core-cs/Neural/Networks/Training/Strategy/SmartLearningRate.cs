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
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Util.Logging;

namespace Encog.Neural.Networks.Training.Strategy
{
    /// <summary>
    /// Attempt to automatically set the learning rate in a learning method that
    /// supports a learning rate.
    /// </summary>
    ///
    public class SmartLearningRate : IStrategy
    {
        /// <summary>
        /// Learning decay rate.
        /// </summary>
        ///
        public const double LearningDecay = 0.99d;

        /// <summary>
        /// The current learning rate.
        /// </summary>
        ///
        private double _currentLearningRate;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double _lastError;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start evaluation.
        /// </summary>
        ///
        private bool _ready;

        /// <summary>
        /// The class that is to have the learning rate set for.
        /// </summary>
        ///
        private ILearningRate _setter;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private IMLTrain _train;

        /// <summary>
        /// The training set size, this is used to pick an initial learning rate.
        /// </summary>
        ///
        private long _trainingSize;

        #region IStrategy Members

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train">The training algorithm.</param>
        public void Init(IMLTrain train)
        {
            _train = train;
            _ready = false;
            _setter = (ILearningRate) train;
            _trainingSize = train.Training.Count;
            _currentLearningRate = 1.0d/_trainingSize;
            EncogLogging.Log(EncogLogging.LevelDebug, "Starting learning rate: "
                                                       + _currentLearningRate);
            _setter.LearningRate = _currentLearningRate;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        ///
        public void PostIteration()
        {
            if (_ready)
            {
                if (_train.Error > _lastError)
                {
                    _currentLearningRate *= LearningDecay;
                    _setter.LearningRate = _currentLearningRate;
                    EncogLogging.Log(EncogLogging.LevelDebug,
                                     "Adjusting learning rate to {}"
                                     + _currentLearningRate);
                }
            }
            else
            {
                _ready = true;
            }
        }

        /// <summary>
        /// Called just before a training iteration.
        /// </summary>
        ///
        public void PreIteration()
        {
            _lastError = _train.Error;
        }

        #endregion
    }
}
