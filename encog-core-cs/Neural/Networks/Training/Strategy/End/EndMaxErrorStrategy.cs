using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Strategy.End
{
    public class EndMaxErrorStrategy : IEndTrainingStrategy
    {
        private double maxError;
        private ITrain train;
        private bool started;

        public EndMaxErrorStrategy(double maxError)
        {
            this.maxError = maxError;
            this.started = false;
        }

        /**
         * {@inheritDoc}
         */
        public bool ShouldStop()
        {
            return this.started && this.train.Error < this.maxError;
        }

        /**
         * {@inheritDoc}
         */
        public void Init(ITrain train)
        {
            this.train = train;
            this.started = false;
        }

        /**
         * {@inheritDoc}
         */
        public void PostIteration()
        {
            this.started = true;
        }

        /**
         * {@inheritDoc}
         */
        public void PreIteration()
        {
        }
    }
}
