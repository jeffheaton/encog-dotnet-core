// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Training.Strategy
{
    /// <summary>
    /// A hybrid stragey allows a secondary training algorithm to be used. Once the
    /// primary algorithm is no longer improving by much, the secondary will be used.
    /// Using simulated annealing in as a secondary to one of the propagation methods
    /// is often a very efficient combination as it can help the propagation method
    /// escape a local minimum. This is particularly true with backpropagation.
    /// </summary>
    public class HybridStrategy : IStrategy
    {

        /// <summary>
        /// The default minimum improvement before we switch to the alternate
        /// training method.
        /// </summary>
        public const double DEFAULT_MIN_IMPROVEMENT = 0.00001;

        /// <summary>
        /// The default number of cycles to tolerate bad improvement for.
        /// </summary>
        public const int DEFAULT_TOLERATE_CYCLES = 10;

        /// <summary>
        /// The default number of cycles to use the alternate training for.
        /// </summary>
        public const int DEFAULT_ALTERNATE_CYCLES = 5;

        /// <summary>
        /// The primary training method.
        /// </summary>
        private ITrain mainTrain;

        /// <summary>
        /// The alternate training method.
        /// </summary>
        private ITrain altTrain;

        /// <summary>
        /// The last improvement.
        /// </summary>
        private double lastImprovement;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        private double lastError;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start 
        /// evaluation.
        /// </summary>
        private bool ready;

        /// <summary>
        /// The last time the alternate training algorithm was used.
        /// </summary>
        private int lastHybrid;

        /// <summary>
        /// The minimum improvement before the alternate training 
        /// algorithm is considered.
        /// </summary>
        private double minImprovement;

        /// <summary>
        /// The number of minimal improvement to tolerate before the
        /// alternate training algorithm is used.
        /// </summary>
        private int tolerateMinImprovement;

        /// <summary>
        /// How many cycles to engage the alternate algorithm for.
        /// </summary>
        private int alternateCycles;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(HybridStrategy));
#endif

        /// <summary>
        /// Construct a hybrid strategy with the default minimum improvement
        /// and toleration cycles.
        /// </summary>
        /// <param name="altTrain">The alternative training strategy.</param>
        public HybridStrategy(ITrain altTrain)
            : this(altTrain, HybridStrategy.DEFAULT_MIN_IMPROVEMENT,
                HybridStrategy.DEFAULT_TOLERATE_CYCLES,
                HybridStrategy.DEFAULT_ALTERNATE_CYCLES)
        {
        }

        /// <summary>
        /// Create a hybrid strategy.
        /// </summary>
        /// <param name="altTrain">The alternate training algorithm.</param>
        /// <param name="minImprovement">The minimum improvement to switch algorithms.</param>
        /// <param name="tolerateMinImprovement">The number of cycles to tolerate the 
        /// minimum improvement for.</param>
        /// <param name="alternateCycles">How many cycles should the alternate 
        /// training algorithm be used for.</param>
        public HybridStrategy(ITrain altTrain, double minImprovement,
                 int tolerateMinImprovement, int alternateCycles)
        {
            this.altTrain = altTrain;
            this.ready = false;
            this.lastHybrid = 0;
            this.minImprovement = minImprovement;
            this.tolerateMinImprovement = tolerateMinImprovement;
            this.alternateCycles = alternateCycles;
        }

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        /// <param name="train">The training algorithm.</param>
        public void Init(ITrain train)
        {
            this.mainTrain = train;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        public void PostIteration()
        {
            if (this.ready)
            {
                double currentError = this.mainTrain.Error;
                this.lastImprovement = (currentError - this.lastError)
                        / this.lastError;
#if logging
                if (this.logger.IsDebugEnabled)
                {
                    this.logger.Debug("Last improvement: " + this.lastImprovement);
                }
#endif

                if ((this.lastImprovement > 0)
                        || (Math.Abs(this.lastImprovement) < this.minImprovement))
                {
                    this.lastHybrid++;

                    if (this.lastHybrid > this.tolerateMinImprovement)
                    {
                        this.lastHybrid = 0;

#if logging
                        if (this.logger.IsDebugEnabled)
                        {
                            this.logger.Debug("Performing hybrid cycle");
                        }
#endif
                        for (int i = 0; i < this.alternateCycles; i++)
                        {
                            this.altTrain.Iteration();
                        }
                    }
                }
            }
            else
            {
                this.ready = true;
            }
        }

        /// <summary>
        /// Called just before a training iteration.
        /// </summary>
        public void PreIteration()
        {
            this.lastError = this.mainTrain.Error;

        }
    }

}
