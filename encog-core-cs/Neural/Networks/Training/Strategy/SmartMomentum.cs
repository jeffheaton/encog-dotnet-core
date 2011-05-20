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
        public const double MIN_IMPROVEMENT = 0.0001d;

        /// <summary>
        /// The maximum value that momentum can go to.
        /// </summary>
        ///
        public const double MAX_MOMENTUM = 4;

        /// <summary>
        /// The starting momentum.
        /// </summary>
        ///
        public const double START_MOMENTUM = 0.1d;

        /// <summary>
        /// How much to increase momentum by.
        /// </summary>
        ///
        public const double MOMENTUM_INCREASE = 0.01d;

        /// <summary>
        /// How many cycles to accept before adjusting momentum.
        /// </summary>
        ///
        public const double MOMENTUM_CYCLES = 10;

        /// <summary>
        /// The current momentum.
        /// </summary>
        ///
        private double currentMomentum;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double lastError;

        /// <summary>
        /// The last improvement in error rate.
        /// </summary>
        ///
        private double lastImprovement;

        /// <summary>
        /// The last momentum.
        /// </summary>
        ///
        private int lastMomentum;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start evaluation.
        /// </summary>
        ///
        private bool ready;

        /// <summary>
        /// The setter used to change momentum.
        /// </summary>
        ///
        private IMomentum setter;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private MLTrain train;

        #region IStrategy Members

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train_0">The training algorithm.</param>
        public void Init(MLTrain train_0)
        {
            train = train_0;
            setter = (IMomentum) train_0;
            ready = false;
            setter.Momentum = 0.0d;
            currentMomentum = 0;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        ///
        public void PostIteration()
        {
            if (ready)
            {
                double currentError = train.Error;
                lastImprovement = (currentError - lastError)
                                  /lastError;
                EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "Last improvement: "
                                                           + lastImprovement);

                if ((lastImprovement > 0)
                    || (Math.Abs(lastImprovement) < MIN_IMPROVEMENT))
                {
                    lastMomentum++;

                    if (lastMomentum > MOMENTUM_CYCLES)
                    {
                        lastMomentum = 0;
                        if (((int) currentMomentum) == 0)
                        {
                            currentMomentum = START_MOMENTUM;
                        }
                        currentMomentum *= (1.0d + MOMENTUM_INCREASE);
                        setter.Momentum = currentMomentum;
                        EncogLogging.Log(EncogLogging.LEVEL_DEBUG,
                                         "Adjusting momentum: " + currentMomentum);
                    }
                }
                else
                {
                    EncogLogging.Log(EncogLogging.LEVEL_DEBUG,
                                     "Setting momentum back to zero.");

                    currentMomentum = 0;
                    setter.Momentum = 0;
                }
            }
            else
            {
                ready = true;
            }
        }

        /// <summary>
        /// Called just before a training iteration.
        /// </summary>
        ///
        public void PreIteration()
        {
            lastError = train.Error;
        }

        #endregion
    }
}
