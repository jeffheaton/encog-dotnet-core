// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Encog.Neural.Networks.Training.Strategy
{
    /// <summary>
    /// Attempt to automatically set a momentum in a training algorithm that supports
    /// momentum.
    /// </summary>
    public class SmartMomentum : IStrategy
    {

        /// <summary>
        /// The minimum improvement to adjust momentum.
        /// </summary>
        public const double MIN_IMPROVEMENT = 0.0001;

        /// <summary>
        /// The maximum value that momentum can go to.
        /// </summary>
        public const double MAX_MOMENTUM = 4;

        /// <summary>
        /// The starting momentum.
        /// </summary>
        public const double START_MOMENTUM = 0.1;

        /// <summary>
        /// How much to increase momentum by.
        /// </summary>
        public const double MOMENTUM_INCREASE = 0.01;

        /// <summary>
        /// How many cycles to accept before adjusting momentum.
        /// </summary>
        public const double MOMENTUM_CYCLES = 10;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        private ITrain train;

        /// <summary>
        /// The setter used to change momentum.
        /// </summary>
        private IMomentum setter;

        /// <summary>
        /// The last improvement in error rate.
        /// </summary>
        private double lastImprovement;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        private double lastError;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start evaluation.
        /// </summary>
        private bool ready;

        /// <summary>
        /// The last momentum.
        /// </summary>
        private int lastMomentum;

        /// <summary>
        /// The current momentum.
        /// </summary>
        private double currentMomentum;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(SmartMomentum));

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        /// <param name="train">The training algorithm.</param>
        public void Init(ITrain train)
        {
            this.train = train;
            this.setter = (IMomentum)train;
            this.ready = false;
            this.setter.Momentum = 0.0;
            this.currentMomentum = 0;

        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        public void PostIteration()
        {
            if (this.ready)
            {
                double currentError = this.train.Error;
                this.lastImprovement = (currentError - this.lastError)
                        / this.lastError;
                if (this.logger.IsDebugEnabled)
                {
                    this.logger.Debug("Last improvement: " + this.lastImprovement);
                }

                if ((this.lastImprovement > 0)
                        || (Math.Abs(this.lastImprovement)
                                < SmartMomentum.MIN_IMPROVEMENT))
                {
                    this.lastMomentum++;

                    if (this.lastMomentum > SmartMomentum.MOMENTUM_CYCLES)
                    {
                        this.lastMomentum = 0;
                        if (((int)this.currentMomentum) == 0)
                        {
                            this.currentMomentum = SmartMomentum.START_MOMENTUM;
                        }
                        this.currentMomentum *=
                            (1.0 + SmartMomentum.MOMENTUM_INCREASE);
                        this.setter.Momentum = this.currentMomentum;
                        if (this.logger.IsDebugEnabled)
                        {
                            this.logger.Debug("Adjusting momentum: " +
                                    this.currentMomentum);
                        }
                    }
                }
                else
                {
                    if (this.logger.IsDebugEnabled)
                    {
                        this.logger.Debug("Setting momentum back to zero.");
                    }

                    this.currentMomentum = 0;
                    this.setter.Momentum = 0;
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
            this.lastError = this.train.Error;
        }

    }

}
