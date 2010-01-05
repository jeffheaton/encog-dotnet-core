// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
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
#if logging
using log4net;
using Encog.Neural.Networks.Structure;
#endif

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

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(Encog));
#endif

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
#if logging
                    if (this.logger.IsDebugEnabled)
                    {
                        this.logger
                                .Debug("Greedy strategy dropped last iteration.");
                    }
#endif
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
