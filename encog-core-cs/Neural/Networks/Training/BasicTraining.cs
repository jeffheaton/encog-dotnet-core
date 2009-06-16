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
