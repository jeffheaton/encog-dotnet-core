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
    /// This strategy will indicate once training is no longer improving the neural
    /// network by a specified amount, over a specified number of cycles. This allows
    /// the program to automatically determine when to stop training.
    /// </summary>
    public class StopTrainingStrategy : IStrategy
    {

        /// <summary>
        /// The default minimum improvement before training stops.
        /// </summary>
        public const double DEFAULT_MIN_IMPROVEMENT = 0.0000001;

        /// <summary>
        /// The default number of cycles to tolerate.
        /// </summary>
        public const int DEFAULT_TOLERATE_CYCLES = 100;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        private ITrain train;

        /// <summary>
        /// Flag to indicate if training should stop.
        /// </summary>
        private bool shouldStop;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start evaluation.
        /// </summary>
        private bool ready;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        private double lastError;

        /// <summary>
        /// The number of cycles to tolerate the minimum improvement.
        /// </summary>
        private double minImprovement;

        /// <summary>
        /// The number of cycles to tolerate the minimum improvement.
        /// </summary>
        private int toleratedCycles;

        /// <summary>
        /// The number of bad training cycles.
        /// </summary>
        private int badCycles;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(StopTrainingStrategy));
#endif

        /// <summary>
        /// Construct the strategy with default options.
        /// </summary>
        public StopTrainingStrategy()
            : this(StopTrainingStrategy.DEFAULT_MIN_IMPROVEMENT,
                StopTrainingStrategy.DEFAULT_TOLERATE_CYCLES)
        {

        }

        /// <summary>
        /// Construct the strategy with the specified parameters.
        /// </summary>
        /// <param name="minImprovement">The minimum accepted improvement.</param>
        /// <param name="toleratedCycles">The number of cycles to tolerate before stopping.</param>
        public StopTrainingStrategy(double minImprovement,
                 int toleratedCycles)
        {
            this.minImprovement = minImprovement;
            this.toleratedCycles = toleratedCycles;
            this.badCycles = 0;
        }

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        /// <param name="train">The training algorithm.</param>
        public void Init(ITrain train)
        {
            this.train = train;
            this.shouldStop = false;
            this.ready = false;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        public void PostIteration()
        {

            if (this.ready)
            {
                if (Math.Abs(this.lastError
                        - this.train.Error) < this.minImprovement)
                {
                    this.badCycles++;
                    if (this.badCycles > this.toleratedCycles)
                    {
                        this.shouldStop = true;
                    }
                }
                else
                {
                    this.badCycles = 0;
                }
            }
            else
            {
                this.ready = true;
            }

            this.lastError = this.train.Error;

        }

        /// <summary>
        /// Called just before a training iteration.
        /// </summary>
        public void PreIteration()
        {
        }

        /// <summary>
        /// True if training should stop.
        /// </summary>
        /// <returns></returns>
        public bool ShouldStop()
        {
            return this.shouldStop;
        }

    }

}
