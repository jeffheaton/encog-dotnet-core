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
using Encog.ML.Train.Strategy.End;

namespace Encog.ML.Train.Strategy
{
    /// <summary>
    /// This strategy will indicate once training is no longer improving the neural
    /// network by a specified amount, over a specified number of cycles. This allows
    /// the program to automatically determine when to stop training.
    /// </summary>
    ///
    public class StopTrainingStrategy : IEndTrainingStrategy
    {
        /// <summary>
        /// The default minimum improvement before training stops.
        /// </summary>
        ///
        public const double DefaultMinImprovement = 0.0000001d;

        /// <summary>
        /// The default number of cycles to tolerate.
        /// </summary>
        ///
        public const int DefaultTolerateCycles = 100;

        /// <summary>
        /// The minimum improvement before training stops.
        /// </summary>
        ///
        private readonly double _minImprovement;

        /// <summary>
        /// The number of cycles to tolerate the minimum improvement.
        /// </summary>
        ///
        private readonly int _toleratedCycles;

        /// <summary>
        /// The number of bad training cycles.
        /// </summary>
        ///
        private int _badCycles;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double _bestError;

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
        /// Flag to indicate if training should stop.
        /// </summary>
        ///
        private bool _shouldStop;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private IMLTrain _train;

        /// <summary>
        /// Construct the strategy with default options.
        /// </summary>
        ///
        public StopTrainingStrategy() : this(DefaultMinImprovement, DefaultTolerateCycles)
        {
        }

        /// <summary>
        /// Construct the strategy with the specified parameters.
        /// </summary>
        ///
        /// <param name="minImprovement">The minimum accepted improvement.</param>
        /// <param name="toleratedCycles">The number of cycles to tolerate before stopping.</param>
        public StopTrainingStrategy(double minImprovement,
                                    int toleratedCycles)
        {
            _minImprovement = minImprovement;
            _toleratedCycles = toleratedCycles;
            _badCycles = 0;
            _bestError = Double.MaxValue;
        }

        #region EndTrainingStrategy Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void Init(IMLTrain train)
        {
            _train = train;
            _shouldStop = false;
            _ready = false;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PostIteration()
        {
            if (_ready)
            {
                if (Math.Abs(_bestError - _train.Error) < _minImprovement)
                {
                    _badCycles++;
                    if (_badCycles > _toleratedCycles)
                    {
                        _shouldStop = true;
                    }
                }
                else
                {
                    _badCycles = 0;
                }
            }
            else
            {
                _ready = true;
            }

            _lastError = _train.Error;
            _bestError = Math.Min(_lastError, _bestError);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PreIteration()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual bool ShouldStop()
        {
            return _shouldStop;
        }

        #endregion
    }
}
