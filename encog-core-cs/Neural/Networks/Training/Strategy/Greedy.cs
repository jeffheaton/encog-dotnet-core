using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Encog.Neural.Networks.Training.Strategy
{
    /// <summary>
    /// A simple greedy strategy. If the last iteration did not improve training,
    /// then discard it. Care must be taken with this strategy, as sometimes a
    /// training algorithm may need to temporarily decrease the error level before
    /// improving it.
    /// </summary>
    public class Greedy : IStrategy
    {

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        private ITrain train;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        private double lastError;

        /// <summary>
        /// The last state of the network, so that we can restore to this
        /// state if needed.
        /// </summary>
        private Double[] lastNetwork;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start 
        /// evaluation.
        /// </summary>
        private bool ready;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(Encog));

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        /// <param name="train">The training algorithm.</param>
        public void Init(ITrain train)
        {
            this.train = train;
            this.ready = false;
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
                    if (this.logger.IsDebugEnabled)
                    {
                        this.logger
                                .Debug("Greedy strategy dropped last iteration.");
                    }
                    this.train.Error = this.lastError;
                    NetworkCODEC.ArrayToNetwork(this.lastNetwork, this.train
                            .Network);
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

            BasicNetwork network = this.train.Network;
            if (network != null)
            {
                this.lastError = this.train.Error;
                this.lastNetwork = NetworkCODEC.NetworkToArray(network);
                this.train.Error = this.lastError;
            }
        }
    }

}
