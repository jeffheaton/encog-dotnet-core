using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Encog.Neural.Networks.Training.Strategy
{
    /// <summary>
    /// The reset strategy will reset the weights if the neural network fails to fall
    /// below a specified error by a specified number of cycles. This can be useful
    /// to throw out initially "bad/hard" random initializations of the weight
    /// matrix.
    /// </summary>
    public class ResetStrategy : IStrategy
    {

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ResetStrategy));

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
        /// Construct a reset strategy.  The error rate must fall
        /// below the required rate in the specified number of cycles,
        /// or the neural network will be reset to random weights and
        /// thresholds.
        /// </summary>
        /// <param name="required">The required error rate.</param>
        /// <param name="cycles">The number of cycles to reach that rate.</param>
        public ResetStrategy(double required, int cycles)
        {
            this.required = required;
            this.cycles = cycles;
            this.badCycleCount = 0;
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
            if (this.train.Error > this.required)
            {
                this.badCycleCount++;
                if (this.badCycleCount > this.cycles)
                {
                    if (this.logger.IsDebugEnabled)
                    {
                        this.logger.Debug("Failed to imrove network, resetting.");
                    }
                    this.train.Network.Reset();
                    this.badCycleCount = 0;
                }
            }
            else
            {
                this.badCycleCount = 0;
            }
        }
    }

}
