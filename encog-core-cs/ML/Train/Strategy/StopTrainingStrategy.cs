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
    public class StopTrainingStrategy : EndTrainingStrategy
    {
        /// <summary>
        /// The default minimum improvement before training stops.
        /// </summary>
        ///
        public const double DEFAULT_MIN_IMPROVEMENT = 0.0000001d;

        /// <summary>
        /// The default number of cycles to tolerate.
        /// </summary>
        ///
        public const int DEFAULT_TOLERATE_CYCLES = 100;

        /// <summary>
        /// The minimum improvement before training stops.
        /// </summary>
        ///
        private readonly double minImprovement;

        /// <summary>
        /// The number of cycles to tolerate the minimum improvement.
        /// </summary>
        ///
        private readonly int toleratedCycles;

        /// <summary>
        /// The number of bad training cycles.
        /// </summary>
        ///
        private int badCycles;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double bestError;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double lastError;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start evaluation.
        /// </summary>
        ///
        private bool ready;

        /// <summary>
        /// Flag to indicate if training should stop.
        /// </summary>
        ///
        private bool shouldStop;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private MLTrain train;

        /// <summary>
        /// Construct the strategy with default options.
        /// </summary>
        ///
        public StopTrainingStrategy() : this(DEFAULT_MIN_IMPROVEMENT, DEFAULT_TOLERATE_CYCLES)
        {
        }

        /// <summary>
        /// Construct the strategy with the specified parameters.
        /// </summary>
        ///
        /// <param name="minImprovement_0">The minimum accepted improvement.</param>
        /// <param name="toleratedCycles_1">The number of cycles to tolerate before stopping.</param>
        public StopTrainingStrategy(double minImprovement_0,
                                    int toleratedCycles_1)
        {
            minImprovement = minImprovement_0;
            toleratedCycles = toleratedCycles_1;
            badCycles = 0;
            bestError = Double.MaxValue;
        }

        #region EndTrainingStrategy Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void Init(MLTrain train_0)
        {
            train = train_0;
            shouldStop = false;
            ready = false;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PostIteration()
        {
            if (ready)
            {
                if (Math.Abs(bestError - train.Error) < minImprovement)
                {
                    badCycles++;
                    if (badCycles > toleratedCycles)
                    {
                        shouldStop = true;
                    }
                }
                else
                {
                    badCycles = 0;
                }
            }
            else
            {
                ready = true;
            }

            lastError = train.Error;
            bestError = Math.Min(lastError, bestError);
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
            return shouldStop;
        }

        #endregion
    }
}
