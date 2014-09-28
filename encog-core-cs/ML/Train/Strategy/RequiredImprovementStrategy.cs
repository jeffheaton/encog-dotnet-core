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
using Encog.Neural.Networks.Training;
using Encog.Util.Logging;

namespace Encog.ML.Train.Strategy
{
    /// <summary>
    /// The reset strategy will reset the weights if the neural network fails to improve by the specified amount over a number of cycles. 
    /// </summary>
    ///
    public class RequiredImprovementStrategy : IStrategy
    {
        /// <summary>
        /// If the error is below this, then never reset.
        /// </summary>
        ///
        private readonly double _acceptableThreshold;

        /// <summary>
        /// The number of cycles to reach the required minimum error.
        /// </summary>
        ///
        private readonly int _cycles;

        /// <summary>
        /// The required minimum error.
        /// </summary>
        ///
        private readonly double _required;

        /// <summary>
        /// How many bad cycles have there been so far.
        /// </summary>
        ///
        private int _badCycleCount;

        /// <summary>
        /// The last error.
        /// </summary>
        ///
        private double _lastError;

        /// <summary>
        /// The method being trained.
        /// </summary>
        private IMLResettable _method;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private IMLTrain _train;

        /// <summary>
        /// Construct a reset strategy. The error rate must fall below the required
        /// rate in the specified number of cycles, or the neural network will be
        /// reset to random weights and bias values.
        /// </summary>
        ///
        /// <param name="required">The required error rate.</param>
        /// <param name="cycles">The number of cycles to reach that rate.</param>
        public RequiredImprovementStrategy(double required, int cycles) : this(required, 0.10d, cycles)
        {
        }

        /// <summary>
        /// Construct a reset strategy. The error rate must fall below the required
        /// rate in the specified number of cycles, or the neural network will be
        /// reset to random weights and bias values.
        /// </summary>
        ///
        /// <param name="required">The required error rate.</param>
        /// <param name="threshold">The accepted threshold, don't reset if error is below this.</param>
        /// <param name="cycles">The number of cycles to reach that rate.</param>
        public RequiredImprovementStrategy(double required,
                                           double threshold, int cycles)
        {
            _lastError = Double.NaN;
            _required = required;
            _cycles = cycles;
            _badCycleCount = 0;
            _acceptableThreshold = threshold;
        }

        /// <summary>
        /// Reset if there is not at least a 1% improvement for 5 cycles. Don't reset
        /// if below 10%.
        /// </summary>
        ///
        /// <param name="cycles"></param>
        public RequiredImprovementStrategy(int cycles) : this(0.01d, 0.10d, cycles)
        {
        }

        #region IStrategy Members

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train">The training algorithm.</param>
        public virtual void Init(IMLTrain train)
        {
            _train = train;

            if (!(train.Method is IMLResettable))
            {
                throw new TrainingError(
                    "To use the required improvement strategy the machine learning method must support MLResettable.");
            }

            _method = (IMLResettable) _train.Method;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        ///
        public virtual void PostIteration()
        {
        }

        /// <summary>
        /// Called just before a training iteration.
        /// </summary>
        ///
        public virtual void PreIteration()
        {
            if (_train.Error > _acceptableThreshold)
            {
                if (!Double.IsNaN(_lastError))
                {
                    double improve = (_lastError - _train.Error);
                    if (improve < _required)
                    {
                        _badCycleCount++;
                        if (_badCycleCount > _cycles)
                        {
                            EncogLogging.Log(EncogLogging.LevelDebug,
                                             "Failed to improve network, resetting.");
                            _method.Reset();
                            _badCycleCount = 0;
                            _lastError = Double.NaN;
                        }
                    }
                    else
                    {
                        _badCycleCount = 0;
                    }
                }
                else
                    _lastError = _train.Error;
            }

            _lastError = Math.Min(_train.Error, _lastError);
        }

        #endregion
    }
}
