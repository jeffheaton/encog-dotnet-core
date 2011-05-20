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
        private readonly double acceptableThreshold;

        /// <summary>
        /// The number of cycles to reach the required minimum error.
        /// </summary>
        ///
        private readonly int cycles;

        /// <summary>
        /// The required minimum error.
        /// </summary>
        ///
        private readonly double required;

        /// <summary>
        /// How many bad cycles have there been so far.
        /// </summary>
        ///
        private int badCycleCount;

        /// <summary>
        /// The last error.
        /// </summary>
        ///
        private double lastError;

        private MLResettable method;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private MLTrain train;

        /// <summary>
        /// Construct a reset strategy. The error rate must fall below the required
        /// rate in the specified number of cycles, or the neural network will be
        /// reset to random weights and bias values.
        /// </summary>
        ///
        /// <param name="required_0">The required error rate.</param>
        /// <param name="cycles_1">The number of cycles to reach that rate.</param>
        public RequiredImprovementStrategy(double required_0, int cycles_1) : this(required_0, 0.10d, cycles_1)
        {
        }

        /// <summary>
        /// Construct a reset strategy. The error rate must fall below the required
        /// rate in the specified number of cycles, or the neural network will be
        /// reset to random weights and bias values.
        /// </summary>
        ///
        /// <param name="required_0">The required error rate.</param>
        /// <param name="threshold">The accepted threshold, don't reset if error is below this.</param>
        /// <param name="cycles_1">The number of cycles to reach that rate.</param>
        public RequiredImprovementStrategy(double required_0,
                                           double threshold, int cycles_1)
        {
            lastError = Double.NaN;
            required = required_0;
            cycles = cycles_1;
            badCycleCount = 0;
            acceptableThreshold = threshold;
        }

        /// <summary>
        /// Reset if there is not at least a 1% improvement for 5 cycles. Don't reset
        /// if below 10%.
        /// </summary>
        ///
        /// <param name="cycles_0"></param>
        public RequiredImprovementStrategy(int cycles_0) : this(0.01d, 0.10d, cycles_0)
        {
        }

        #region IStrategy Members

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train_0">The training algorithm.</param>
        public virtual void Init(MLTrain train_0)
        {
            train = train_0;

            if (!(train_0.Method is MLResettable))
            {
                throw new TrainingError(
                    "To use the required improvement strategy the machine learning method must support MLResettable.");
            }

            method = (MLResettable) train.Method;
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
            if (train.Error > acceptableThreshold)
            {
                if (!Double.IsNaN(lastError))
                {
                    double improve = (lastError - train.Error);
                    if (improve < required)
                    {
                        badCycleCount++;
                        if (badCycleCount > cycles)
                        {
                            EncogLogging.Log(EncogLogging.LEVEL_DEBUG,
                                             "Failed to improve network, resetting.");
                            method.Reset();
                            badCycleCount = 0;
                            lastError = Double.NaN;
                        }
                    }
                    else
                    {
                        badCycleCount = 0;
                    }
                }
                else
                    lastError = train.Error;
            }

            lastError = Math.Min(train.Error, lastError);
        }

        #endregion
    }
}
