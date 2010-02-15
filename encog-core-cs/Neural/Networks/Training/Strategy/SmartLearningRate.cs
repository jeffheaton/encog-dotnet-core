// Encog(tm) Artificial Intelligence Framework v2.3
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
using Encog.Neural.NeuralData;

#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Training.Strategy
{
    /// <summary>
    /// Attempt to automatically set the learning rate in a learning method that
    /// supports a learning rate.
    /// </summary>
    public class SmartLearningRate : IStrategy
    {

        /// <summary>
        /// Learning decay rate.
        /// </summary>
        public const double LEARNING_DECAY = 0.99;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        private ITrain train;

        /// <summary>
        /// The class that is to have the learning rate set for.
        /// </summary>
        private ILearningRate setter;

        /// <summary>
        /// The current learning rate.
        /// </summary>
        private double currentLearningRate;

        /// <summary>
        /// The training set size, this is used to pick an initial learning rate.
        /// </summary>
        private long trainingSize;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        private double lastError;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start evaluation.
        /// </summary>
        private bool ready;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(SmartLearningRate));
#endif

        /// <summary>
        /// Determine the training size.
        /// </summary>
        /// <returns>The training size.</returns>
        private long DetermineTrainingSize()
        {
            long result = 0;
            foreach (
             INeuralDataPair pair in this.train.Training)
            {
                result++;
            }
            return result;
        }

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        /// <param name="train">The training algorithm.</param>
        public void Init(ITrain train)
        {
            this.train = train;
            this.ready = false;
            this.setter = (ILearningRate)train;
            this.trainingSize = DetermineTrainingSize();
            this.currentLearningRate = 1.0 / this.trainingSize;
#if logging
            if (this.logger.IsInfoEnabled)
            {
                this.logger.Info("Starting learning rate: " +
                        this.currentLearningRate);
            }
#endif
            this.setter.LearningRate = this.currentLearningRate;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        public void PostIteration()
        {
            if (this.ready)
            {
                if (this.train.Error > this.lastError)
                {
                    this.currentLearningRate *= SmartLearningRate.LEARNING_DECAY;
                    this.setter.LearningRate = this.currentLearningRate;
#if logging
                    if (this.logger.IsInfoEnabled)
                    {
                        this.logger.Info("Adjusting learning rate to " +
                                this.currentLearningRate);
                    }
#endif
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
