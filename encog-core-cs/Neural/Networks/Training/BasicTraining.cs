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
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// An abstract class that implements basic training for most training
    /// algorithms. Specifically training strategies can be added to enhance the
    /// training.
    /// </summary>
    public abstract class BasicTraining : ITrain
    {
        /// <summary>
        /// The training strategies to use. 
        /// </summary>
        private IList<IStrategy> strategies = new List<IStrategy>();

        /// <summary>
        /// The training data.
        /// </summary>
        private INeuralDataSet training;

        /// <summary>
        /// The current error rate.
        /// </summary>
        private double error;

        /// <summary>
        /// Training strategies can be added to improve the training results. There
        /// are a number to choose from, and several can be used at once.
        /// </summary>
        /// <param name="strategy">The strategy to add.</param>
        public void AddStrategy(IStrategy strategy)
        {
            strategy.Init(this);
            this.strategies.Add(strategy);
        }

        /// <summary>
        /// Get the current error percent from the training.
        /// </summary>
        public double Error
        {
            get
            {
                return this.error;
            }
            set
            {
                this.error = value;
            }
        }

        /// <summary>
        /// The strategies to use.
        /// </summary>
        public IList<IStrategy> Strategies
        {
            get
            {
                return this.strategies;
            }
        }

        /// <summary>
        /// The training data to use.
        /// </summary>
        public INeuralDataSet Training
        {
            get
            {
                return this.training;
            }
            set
            {
                this.training = value;
            }
        }

        /// <summary>
        /// Call the strategies after an iteration.
        /// </summary>
        public void PostIteration()
        {
            foreach (IStrategy strategy in this.strategies)
            {
                strategy.PostIteration();
            }
        }

        /// <summary>
        /// Call the strategies before an iteration.
        /// </summary>
        public void PreIteration()
        {
            foreach (IStrategy strategy in this.strategies)
            {
                strategy.PreIteration();
            }
        }

        /// <summary>
        /// Get the current best network from the training.
        /// </summary>
        public abstract BasicNetwork Network
        {
            get;
        }

        /// <summary>
        /// Perform one iteration of training.
        /// </summary>
        public abstract void Iteration();
    }
}
