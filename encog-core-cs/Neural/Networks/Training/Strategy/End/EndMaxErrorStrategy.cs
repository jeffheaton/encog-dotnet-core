using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Strategy.End
{
    /// <summary>
    /// A strategy that ends when a maximum error is reached.
    /// </summary>
    public class EndMaxErrorStrategy : IEndTrainingStrategy
    {
        /// <summary>
        /// The maximum error.
        /// </summary>
        private double maxError;

        /// <summary>
        /// The trainer.
        /// </summary>
        private ITrain train;

        /// <summary>
        /// Has the training started?
        /// </summary>
        private bool started;

        /// <summary>
        /// Construct a max error ending strategy.
        /// </summary>
        /// <param name="maxError">The maximum error.</param>
        public EndMaxErrorStrategy(double maxError)
        {
            this.maxError = maxError;
            this.started = false;
        }

        /// <inheritdoc/>
        public bool ShouldStop()
        {
            return this.started && this.train.Error < this.maxError;
        }

        /// <inheritdoc/>
        public void Init(ITrain train)
        {
            this.train = train;
            this.started = false;
        }

        /// <inheritdoc/>
        public void PostIteration()
        {
            this.started = true;
        }

        /// <inheritdoc/>
        public void PreIteration()
        {
        }
    }
}
