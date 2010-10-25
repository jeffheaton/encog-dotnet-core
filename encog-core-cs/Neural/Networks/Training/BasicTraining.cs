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
using Encog.Neural.NeuralData;
using Encog.Cloud;

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
        /// The current iteration.
        /// </summary>
        private int iteration;

#if !SILVERLIGHT
        /// <summary>
        /// The Encog cloud to use.
        /// </summary>
        public EncogCloud Cloud { get; set; }

        private TrainingStatusUtility statusUtil;
#endif

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
            this.iteration++;
#if !SILVERLIGHT
            if (this.statusUtil != null)
            {
                this.statusUtil.Update();
            }
            else
            {
                if (this.Cloud != null)
                {
                    this.statusUtil = new TrainingStatusUtility(this.Cloud, this);
                    this.statusUtil.Update();
                }
            }
#endif


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
        /// Should be called after training has completed and the iteration method
        /// will not be called any further.
        /// </summary>
        public virtual void FinishTraining()
        {
#if !SILVERLIGHT
            if (this.statusUtil != null)
            {
                this.statusUtil.Finish();
                this.statusUtil = null;
            }
#endif
        }

        /// <summary>
        /// Perform one iteration of training.
        /// </summary>
        public abstract void Iteration();


        /// <summary>
        /// True if training can progress no further.  Not all training methods use this, as not all can tell when they are done.          
        /// </summary>
        public bool TrainingDone
        {
            get
            {
                return false;
            }
        }
        
        /// <summary>
        /// Perform the specified number of training iterations. This is a basic implementation 
        /// that just calls iteration the specified number of times.  However, some training 
        /// methods, particularly with the GPU, benefit greatly by calling with higher numbers than 1. 
        /// </summary>
        /// <param name="count">The number of training iterations.</param>
        public virtual void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }

        /// <summary>
        /// The current iteration.
        /// </summary>
        public int CurrentIteration
        {
            get
            {
                return iteration;
            }
            set
            {
                this.iteration = value;
            }
        }
    }
}
