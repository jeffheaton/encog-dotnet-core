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
using Encog.Util.Logging;

namespace Encog.ML.Train.Strategy
{
    /// <summary>
    /// A hybrid stragey allows a secondary training algorithm to be used. Once the
    /// primary algorithm is no longer improving by much, the secondary will be used.
    /// Using simulated annealing in as a secondary to one of the propagation methods
    /// is often a very efficient combination as it can help the propagation method
    /// escape a local minimum. This is particularly true with backpropagation.
    /// </summary>
    ///
    public class HybridStrategy : IStrategy
    {
        /// <summary>
        /// The default minimum improvement before we switch to the alternate
        /// training method.
        /// </summary>
        ///
        public const double DEFAULT_MIN_IMPROVEMENT = 0.00001d;

        /// <summary>
        /// The default number of cycles to tolerate bad improvement for.
        /// </summary>
        ///
        public const int DEFAULT_TOLERATE_CYCLES = 10;

        /// <summary>
        /// The default number of cycles to use the alternate training for.
        /// </summary>
        ///
        public const int DEFAULT_ALTERNATE_CYCLES = 5;

        /// <summary>
        /// The alternate training method.
        /// </summary>
        ///
        private readonly MLTrain altTrain;

        /// <summary>
        /// How many cycles to engage the alternate algorithm for.
        /// </summary>
        ///
        private readonly int alternateCycles;

        /// <summary>
        /// The minimum improvement before the alternate training 
        /// algorithm is considered.
        /// </summary>
        ///
        private readonly double minImprovement;

        /// <summary>
        /// The number of minimal improvement to tolerate before the
        /// alternate training algorithm is used.
        /// </summary>
        ///
        private readonly int tolerateMinImprovement;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double lastError;

        /// <summary>
        /// The last time the alternate training algorithm was used.
        /// </summary>
        ///
        private int lastHybrid;

        /// <summary>
        /// The last improvement.
        /// </summary>
        ///
        private double lastImprovement;

        /// <summary>
        /// The primary training method.
        /// </summary>
        ///
        private MLTrain mainTrain;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start 
        /// evaluation.
        /// </summary>
        ///
        private bool ready;

        /// <summary>
        /// Construct a hybrid strategy with the default minimum improvement
        /// and toleration cycles.
        /// </summary>
        ///
        /// <param name="altTrain_0">The alternative training strategy.</param>
        public HybridStrategy(MLTrain altTrain_0)
            : this(altTrain_0, DEFAULT_MIN_IMPROVEMENT, DEFAULT_TOLERATE_CYCLES, DEFAULT_ALTERNATE_CYCLES)
        {
        }

        /// <summary>
        /// Create a hybrid strategy.
        /// </summary>
        ///
        /// <param name="altTrain_0">The alternate training algorithm.</param>
        /// <param name="minImprovement_1">The minimum improvement to switch algorithms.</param>
        /// <param name="tolerateMinImprovement_2"></param>
        /// <param name="alternateCycles_3"></param>
        public HybridStrategy(MLTrain altTrain_0, double minImprovement_1,
                              int tolerateMinImprovement_2, int alternateCycles_3)
        {
            altTrain = altTrain_0;
            ready = false;
            lastHybrid = 0;
            minImprovement = minImprovement_1;
            tolerateMinImprovement = tolerateMinImprovement_2;
            alternateCycles = alternateCycles_3;
        }

        #region IStrategy Members

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train">The training algorithm.</param>
        public virtual void Init(MLTrain train)
        {
            mainTrain = train;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        ///
        public virtual void PostIteration()
        {
            if (ready)
            {
                double currentError = mainTrain.Error;
                lastImprovement = (currentError - lastError)
                                  /lastError;
                EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "Last improvement: "
                                                           + lastImprovement);

                if ((lastImprovement > 0)
                    || (Math.Abs(lastImprovement) < minImprovement))
                {
                    lastHybrid++;

                    if (lastHybrid > tolerateMinImprovement)
                    {
                        lastHybrid = 0;

                        EncogLogging.Log(EncogLogging.LEVEL_DEBUG,
                                         "Performing hybrid cycle");

                        for (int i = 0; i < alternateCycles; i++)
                        {
                            altTrain.Iteration();
                        }
                    }
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
        public virtual void PreIteration()
        {
            lastError = mainTrain.Error;
        }

        #endregion
    }
}
