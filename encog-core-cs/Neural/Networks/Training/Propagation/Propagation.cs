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
using Encog.Util;
using Encog.Neural.Data;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks.Layers;
using Encog.Util.Logging;
#if logging
using log4net;
#endif
namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Implements basic functionality that is needed by each of the propagation
    /// methods. The specifics of each of the propagation methods is implemented
    /// inside of the PropagationMethod interface implementors.
    /// </summary>
    public class Propagation : BasicTraining
    {
        /// <summary>
        /// The batch size. Defaults to the max size of an integer, which means
        /// update once per iteration.
        /// 
        /// The batch size is the frequency with which the weights are updated per
        /// iteration. Setting it to the size of the training set means one update
        /// per iteration. Setting this to a lower number may improve training
        /// efficiency at the cost of processing time.
        /// 
        /// If you do not want to use batch training, specify a value of 1, then the
        /// weights will be updated on each iteration, which is online training.
        /// </summary>
        private int batchSize = int.MaxValue;

        /// <summary>
        /// The propagation utility to use.
        /// </summary>
	private PropagationUtil propagationUtil;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(Propagation));
#endif

        /// <summary>
        /// Construct a propagation trainer.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="method">The propagation method to use.</param>
        /// <param name="training">The training data to use.</param>
        public Propagation(BasicNetwork network,
                 IPropagationMethod method, INeuralDataSet training)
        {
            this.propagationUtil = new PropagationUtil(network, method);
            this.Training = training;
        }

        
        /// <summary>
        /// Perform one iteration of training.
        /// 
        /// Note: if you get a StackOverflowError while training, then you have
        /// endless recurrent loops. Try inserting no trainable synapses on one side
        /// of the loop.
        /// </summary>
        public override void Iteration()
        {
#if logging
            if (this.logger.IsInfoEnabled)
            {
                this.logger.Info("Beginning propagation iteration");
            }
#endif
            PreIteration();

            ErrorCalculation errorCalculation = new ErrorCalculation();
            int processedCount = 0;

            foreach (INeuralDataPair pair in this.Training)
            {
#if logging
                if (this.logger.IsDebugEnabled)
                {
                    this.logger.Debug(
                            "Backpropagation training on: input=" + pair.Input + ",ideal=" + pair.Ideal);
                }
#endif
                INeuralData actual = this.propagationUtil.ForwardPass(pair.Input);

                errorCalculation.UpdateError(actual.Data, pair.Ideal.Data);
                this.propagationUtil.BackwardPass(pair.Ideal);

                processedCount++;
                if (processedCount >= this.batchSize)
                {
                    processedCount = 0;
                    this.propagationUtil.Method.Learn();
                }

            }

            if (processedCount != 0)
            {
                this.propagationUtil.Method.Learn();
            }


            this.Error = errorCalculation.CalculateRMS();

            PostIteration();
        }

        /// <summary>
        /// Get the current best neural network.
        /// </summary>
        public override BasicNetwork Network
        {
            get
            {
                return this.propagationUtil.Network;
            }
        }
    }
}
