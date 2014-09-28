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
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Util.Logging;

namespace Encog.Neural.Networks.Training.Strategy
{
    /// <summary>
    /// Attempt to automatically set a momentum in a training algorithm that supports
    /// momentum.
    /// </summary>
    ///
    public class SmartMomentum : IStrategy
    {
        /// <summary>
        /// The minimum improvement to adjust momentum.
        /// </summary>
        ///
        public const double MinImprovement = 0.0001d;

        /// <summary>
        /// The maximum value that momentum can go to.
        /// </summary>
        ///
        public const double MaxMomentum = 4;

        /// <summary>
        /// The starting momentum.
        /// </summary>
        ///
        public const double StartMomentum = 0.1d;

        /// <summary>
        /// How much to increase momentum by.
        /// </summary>
        ///
        public const double MomentumIncrease = 0.01d;

        /// <summary>
        /// How many cycles to accept before adjusting momentum.
        /// </summary>
        ///
        public const double MomentumCycles = 10;

        /// <summary>
        /// The current momentum.
        /// </summary>
        ///
        private double _currentMomentum;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double _lastError;

        /// <summary>
        /// The last improvement in error rate.
        /// </summary>
        ///
        private double _lastImprovement;

        /// <summary>
        /// The last momentum.
        /// </summary>
        ///
        private int _lastMomentum;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start evaluation.
        /// </summary>
        ///
        private bool _ready;

        /// <summary>
        /// The setter used to change momentum.
        /// </summary>
        ///
        private IMomentum _setter;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private IMLTrain _train;

        #region IStrategy Members

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train_0">The training algorithm.</param>
        public void Init(IMLTrain train_0)
        {
            _train = train_0;
            _setter = (IMomentum) train_0;
            _ready = false;
            _setter.Momentum = 0.0d;
            _currentMomentum = 0;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        ///
        public void PostIteration()
        {
            if (_ready)
            {
                double currentError = _train.Error;
                _lastImprovement = (currentError - _lastError)
                                  /_lastError;
                EncogLogging.Log(EncogLogging.LevelDebug, "Last improvement: "
                                                           + _lastImprovement);

                if ((_lastImprovement > 0)
                    || (Math.Abs(_lastImprovement) < MinImprovement))
                {
                    _lastMomentum++;

                    if (_lastMomentum > MomentumCycles)
                    {
                        _lastMomentum = 0;
                        if (((int) _currentMomentum) == 0)
                        {
                            _currentMomentum = StartMomentum;
                        }
                        _currentMomentum *= (1.0d + MomentumIncrease);
                        _setter.Momentum = _currentMomentum;
                        EncogLogging.Log(EncogLogging.LevelDebug,
                                         "Adjusting momentum: " + _currentMomentum);
                    }
                }
                else
                {
                    EncogLogging.Log(EncogLogging.LevelDebug,
                                     "Setting momentum back to zero.");

                    _currentMomentum = 0;
                    _setter.Momentum = 0;
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
