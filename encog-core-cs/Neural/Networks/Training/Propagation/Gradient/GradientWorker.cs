// Encog(tm) Artificial Intelligence Framework v2.3
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
using Encog.Neural.Data;
using Encog.Util.Concurrency;

namespace Encog.Neural.Networks.Training.Propagation.Gradient
{
    /// <summary>
    /// A worker handles one thread. Used to allow the gradient calculation process
    /// to run multithreaded. Even if running in single threaded mode, a single
    /// worker is created and run by the main thread.
    /// </summary>
    public class GradientWorker: IEncogTask
    {

        /// <summary>
        /// The high index point in the training data to be used by this individual
        /// worker.
        /// </summary>
        private int high;

        /// <summary>
        /// The low index point in the training data to be used by this individual
        /// worker.
        /// </summary>
        private int low;

        /// <summary>
        /// The owner of this worker.
        /// </summary>
        private CalculateGradient owner;

        /// <summary>
        /// The network being used by this worker.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The training set used by this worker.
        /// </summary>
        private INeuralDataSet training;

        /// <summary>
        /// The gradient util used by this worker.
        /// </summary>
        private GradientUtil gradient;

        /// <summary>
        /// Construct a worker. 
        /// </summary>
        /// <param name="owner">The owner of this worker.</param>
        /// <param name="training">The training set that this worker is to use.</param>
        /// <param name="low">The low element in the training set.</param>
        /// <param name="high">The high element in the training set.</param>
        public GradientWorker(CalculateGradient owner,
                 INeuralDataSet training, int low, int high)
        {
            this.owner = owner;
            this.high = high;
            this.low = low;
            this.network = (BasicNetwork)owner.Network.Clone();
            this.training = training;
            this.gradient = new GradientUtil(this.network);
        }

        /// <summary>
        /// The number of training elements ot be processed by this worker.
        /// </summary>
        public int Count
        {
            get
            {
                return this.gradient.Count;
            }
        }

        /// <summary>
        /// The overall error for this worker.
        /// </summary>
        public double Error
        {
            get
            {
                return this.gradient.Error;
            }
        }

        /// <summary>
        /// The gradients calculated for this worker.
        /// </summary>
        public double[] Errors
        {
            get
            {
                return this.gradient.Errors;
            }
        }

        /// <summary>
        /// The network to calculate gradients for.
        /// </summary>
        public BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// The main loop for this thread.
        /// </summary>
        public void Run()
        {
            double[] weights = this.owner.Weights;
            INeuralDataPair pair = this.owner.CreatePair();

            if ((this.training is IIndexable) && (this.high != this.low))
            {
                IIndexable t2 = (IIndexable)this.training;
                this.gradient.Reset(weights);
                for (int i = this.low; i <= this.high; i++)
                {
                    t2.GetRecord(i, pair);
                    this.gradient.Calculate(pair.Input, pair.Ideal);
                }
            }
            else
            {
                this.gradient.Calculate(this.training, weights);
            }
        }
    }
}
