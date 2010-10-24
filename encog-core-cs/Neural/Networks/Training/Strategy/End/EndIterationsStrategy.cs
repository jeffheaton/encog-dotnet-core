using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Strategy.End
{
    /// <summary>
    /// End after a number of iterations.
    /// </summary>
    public class EndIterationsStrategy : IEndTrainingStrategy
    {
        /// <summary>
        /// The number of iterations to end after.
        /// </summary>
        private int maxIterations;

        /// <summary>
        /// The current iteration.
        /// </summary>
        private int currentIteration;

        /// <summary>
        /// The trainer to use.
        /// </summary>
        private ITrain train;

        /// <summary>
        /// Construct a strategy.
        /// </summary>
        /// <param name="maxIterations">The number of iterations.</param>
        public EndIterationsStrategy(int maxIterations)
        {
            this.maxIterations = maxIterations;
            this.currentIteration = 0;
        }


        /// <inheritdoc/>
        public bool ShouldStop()
        {
            return (this.currentIteration >= this.maxIterations);
        }

        /// <inheritdoc/>
        public void Init(ITrain train)
        {
            this.train = train;
        }

        /// <inheritdoc/>
        public void PostIteration()
        {
            this.currentIteration = this.train.CurrentIteration;
        }

        /// <inheritdoc/>
        public void PreIteration()
        {
        }
    }
}
