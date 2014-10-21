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
        public const double DefaultMinImprovement = 0.00001d;

        /// <summary>
        /// The default number of cycles to tolerate bad improvement for.
        /// </summary>
        ///
        public const int DefaultTolerateCycles = 10;

        /// <summary>
        /// The default number of cycles to use the alternate training for.
        /// </summary>
        ///
        public const int DefaultAlternateCycles = 5;

        /// <summary>
        /// The alternate training method.
        /// </summary>
        ///
        private readonly IMLTrain _altTrain;

        /// <summary>
        /// How many cycles to engage the alternate algorithm for.
        /// </summary>
        ///
        private readonly int _alternateCycles;

        /// <summary>
        /// The minimum improvement before the alternate training 
        /// algorithm is considered.
        /// </summary>
        ///
        private readonly double _minImprovement;

        /// <summary>
        /// The number of minimal improvement to tolerate before the
        /// alternate training algorithm is used.
        /// </summary>
        ///
        private readonly int _tolerateMinImprovement;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double _lastError;

        /// <summary>
        /// The last time the alternate training algorithm was used.
        /// </summary>
        ///
        private int _lastHybrid;

        /// <summary>
        /// The last improvement.
        /// </summary>
        ///
        private double _lastImprovement;

        /// <summary>
        /// The primary training method.
        /// </summary>
        ///
        private IMLTrain _mainTrain;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start 
        /// evaluation.
        /// </summary>
        ///
        private bool _ready;

        /// <summary>
        /// Construct a hybrid strategy with the default minimum improvement
        /// and toleration cycles.
        /// </summary>
        ///
        /// <param name="altTrain">The alternative training strategy.</param>
        public HybridStrategy(IMLTrain altTrain)
            : this(altTrain, DefaultMinImprovement, DefaultTolerateCycles, DefaultAlternateCycles)
        {
        }

        /// <summary>
        /// Create a hybrid strategy.
        /// </summary>
        ///
        /// <param name="altTrain">The alternate training algorithm.</param>
        /// <param name="minImprovement">The minimum improvement to switch algorithms.</param>
        /// <param name="tolerateMinImprovement"></param>
        /// <param name="alternateCycles"></param>
        public HybridStrategy(IMLTrain altTrain, double minImprovement,
                              int tolerateMinImprovement, int alternateCycles)
        {
            _altTrain = altTrain;
            _ready = false;
            _lastHybrid = 0;
            _minImprovement = minImprovement;
            _tolerateMinImprovement = tolerateMinImprovement;
            _alternateCycles = alternateCycles;
        }

        #region IStrategy Members

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train">The training algorithm.</param>
        public virtual void Init(IMLTrain train)
        {
            _mainTrain = train;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        ///
        public virtual void PostIteration()
        {
            if (_ready)
            {
                double currentError = _mainTrain.Error;
                _lastImprovement = (currentError - _lastError)
                                  /_lastError;
                EncogLogging.Log(EncogLogging.LevelDebug, "Last improvement: "
                                                           + _lastImprovement);

                if ((_lastImprovement > 0)
                    || (Math.Abs(_lastImprovement) < _minImprovement))
                {
                    _lastHybrid++;

                    if (_lastHybrid > _tolerateMinImprovement)
                    {
                        _lastHybrid = 0;

                        EncogLogging.Log(EncogLogging.LevelDebug,
                                         "Performing hybrid cycle");

                        for (int i = 0; i < _alternateCycles; i++)
                        {
                            _altTrain.Iteration();
                        }
                    }
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
        public virtual void PreIteration()
        {
            _lastError = _mainTrain.Error;
        }

        #endregion
    }
}
