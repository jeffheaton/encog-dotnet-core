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
using Encog.MathUtil;
using Encog.Neural.Data;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Gradient;
using Encog.Neural.Networks.Structure;

#if logging
using log4net;
using Encog.Util;
using Encog.Neural.Networks.Flat;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
#endif

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Implements basic functionality that is needed by each of the propagation
    /// methods. The specifics of each of the propagation methods is implemented
    /// inside of the PropagationMethod interface implementors.
    /// </summary>
    public abstract class Propagation : BasicTraining
    {
        /// <summary>
        /// The number of threads to use.
        /// </summary>
        public int NumThreads { get; set; }

        /// <summary>
        /// The network.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// Construct a propagation object.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <param name="training">The training set.</param>
        public Propagation(BasicNetwork network, INeuralDataSet training)
            : base()
        {
            this.network = network;
            this.Training = training;
            this.EnforcedCLRatio = 1.0;

            if (this.Training is IIndexable && ValidateForFlat.CanBeFlat(this.network) == null)
            {
                this.AttemptFlatten = true;
            }
            else
            {
                this.AttemptFlatten = false;
            }
        }

        /// <summary>
        /// True if this training can be continued.
        /// </summary>
        public virtual bool CanContinue
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The network.
        /// </summary>
        public override BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// The current flat network we are using for training, or null for none.
        /// </summary>
        public FlatNetwork CurrentFlatNetwork { get; set; }

        /// <summary>
        /// The current flat trainer we are using, or null for none.
        /// </summary>
        public TrainFlatNetworkMulti FlatTraining { get; set; }

        /// <summary>
        /// Should we attempt to flatten the network?  Usually you want this to be true, 
        /// a flat network can train much faster.  However, we can not always use flat networks.  
        /// See the FlatNetwork class for more info.
        /// </summary>
        public bool AttemptFlatten { get; set; }

        /// <summary>
        /// The enforced CL ratio.
        /// </summary>
        public double EnforcedCLRatio { get; set; }

        /// <summary>
        /// Determine if this specified training continuation object is valid for
        /// this training method.
        /// </summary>
        /// <param name="state">The training continuation object to check.</param>
        /// <returns>True if the continuation object is valid.</returns>
        public virtual bool IsValidResume(TrainingContinuation state)
        {
            return false;
        }

        /// <summary>
        /// Attempt to flatten the network.
        /// </summary>
        private void ProcessFlatten()
        {
            if (this.AttemptFlatten && this.CurrentFlatNetwork==null)
            {
                if (this.Training is IIndexable && ValidateForFlat.CanBeFlat(this.network)==null )
                {
                    this.CurrentFlatNetwork = new FlatNetwork(network);

                    if (this is ResilientPropagation)
                    {
                        this.FlatTraining = new TrainFlatNetworkResilient(CurrentFlatNetwork, Training, EnforcedCLRatio);
                    }
                    else if (this is Backpropagation)
                    {
                        Backpropagation b = (Backpropagation)this;
                        this.FlatTraining = new TrainFlatNetworkBackPropagation(CurrentFlatNetwork, Training, b.LearningRate, b.Momentum, EnforcedCLRatio);
                    }
                    else if (this is ManhattanPropagation)
                    {
                        ManhattanPropagation m = (ManhattanPropagation)this;
                        this.FlatTraining = new TrainFlatNetworkManhattan(CurrentFlatNetwork, Training, m.LearningRate, EnforcedCLRatio);
                    }
                    else
                    {
                        this.CurrentFlatNetwork = null;
                        this.AttemptFlatten = false;
                        return;
                    }

                    this.FlatTraining.NumThreads = NumThreads;
                }
                else
                    this.AttemptFlatten = false;
            }
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {
            try
            {
                PreIteration();
                ProcessFlatten();

                if (this.FlatTraining == null)
                {
                    CalculateGradient prop = new CalculateGradient(Network, Training, NumThreads);
                    double[] weights = NetworkCODEC.NetworkToArray(Network);
                    prop.Calculate(weights);

                    PerformIteration(prop, weights);

                    NetworkCODEC.ArrayToNetwork(weights, Network);
                    Error = prop.Error;
                }
                else
                {
                    EncogArray.ArrayCopy(
                        NetworkCODEC.NetworkToArray(this.network),
                        CurrentFlatNetwork.Weights);
                    this.FlatTraining.Iteration();
                    this.Error = FlatTraining.Error;
                    NetworkCODEC.ArrayToNetwork(CurrentFlatNetwork.Weights, this.network);
                }

                PostIteration();
            }
            catch (IndexOutOfRangeException )
            {
                EncogValidate.ValidateNetworkForTraining(network, Training);
            }
        }

        /// <summary>
        /// Pause the training to continue later.
        /// </summary>
        /// <returns>A training continuation object.</returns>
        public virtual TrainingContinuation Pause()
        {
            throw new TrainingError("This training type does not support pause.");
        }



        /// <summary>
        /// Perform an iteration. This is implemented for each of the propagation
        /// method types.
        /// </summary>
        /// <param name="prop">The gradients.</param>
        /// <param name="weights">The weights.</param>
        public abstract void PerformIteration(CalculateGradient prop,
                double[] weights);

        /// <summary>
        /// Resume training.
        /// </summary>
        /// <param name="state">The training continuation object to use to continue.</param>
        public virtual void Resume(TrainingContinuation state)
        {
            throw new TrainingError("This training type does not support resume.");
        }
    }
}
