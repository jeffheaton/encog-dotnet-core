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

namespace Encog.Neural.Networks.Training.Strategy
{
    /// <summary>
    /// This reset strategy will reset the weights if the neural network fails to improve 
    /// by the specified amount over a number of cycles.
    /// </summary>
    public class RequiredImprovementStrategy : IStrategy
    {

        /// <summary>
        /// The required minimum error.
        /// </summary>
        private double required;

        /// <summary>
        /// The number of cycles to reach the required minimum error.
        /// </summary>
        private int cycles;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        private ITrain train;

        /// <summary>
        /// How many bad cycles have there been so far.
        /// </summary>
        private int badCycleCount;

        /// <summary>
        /// The last error.
        /// </summary>
        private double lastError = Double.NaN;

        /// <summary>
        /// If the error is below this, then never reset.
        /// </summary>
        private double acceptableThreshold;


        /// <summary>
        /// Construct a reset strategy. The error rate must fall below the required
        /// rate in the specified number of cycles, or the neural network will be
        /// reset to random weights and bias values. 
        /// </summary>
        /// <param name="required">The required error rate.</param>
        /// <param name="cycles">The number of cycles to reach that rate.</param>
        public RequiredImprovementStrategy(double required, int cycles)
            : this(required, 0.10, cycles)
        {

        }

     
        /// <summary>
        /// Construct a reset strategy. The error rate must fall below the required
        /// rate in the specified number of cycles, or the neural network will be
        /// reset to random weights and bias values.
        /// </summary>
        /// <param name="required">The required error rate.</param>
        /// <param name="threshold">The accepted threshold, don't reset if error is below this.</param>
        /// <param name="cycles">The number of cycles to reach that rate.</param>
        public RequiredImprovementStrategy(double required, double threshold,
                int cycles)
        {
            this.required = required;
            this.cycles = cycles;
            this.badCycleCount = 0;
            this.acceptableThreshold = threshold;
        }

        /// <summary>
        /// Reset if there is not at least a 1% improvement for 5 cycles. Don't reset
        /// if below 10%.
        /// </summary>
        /// <param name="cycles">How many cycles to tolerate.</param>
        public RequiredImprovementStrategy(int cycles)
            : this(0.01, 0.10, 5)
        {

        }

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        /// <param name="train">The training algorithm.</param>
        public void Init(ITrain train)
        {
            this.train = train;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        public void PostIteration()
        {

        }

        /// <summary>
        /// Called just before a training iteration.
        /// </summary>
        public void PreIteration()
        {

            if (train.Error > this.acceptableThreshold)
            {
                if (!Double.IsNaN(lastError))
                {
                    double improve = (lastError - train.Error);
                    if (improve < this.required)
                    {
                        this.badCycleCount++;
                        if (this.badCycleCount > this.cycles)
                        {
                            this.train.Network.Reset();
                            this.badCycleCount = 0;
                            this.lastError = Double.NaN;
                        }
                    }
                    else
                    {
                        this.badCycleCount = 0;
                    }
                }
                else
                    lastError = train.Error;
            }

            lastError = Math.Min(this.train.Error, lastError);
        }
    }
}
