// Encog(tm) Artificial Intelligence Framework v2.5
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

#if logging
using log4net;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil;
using Encog.Neural.Data;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Structure;
using Encog.Util;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Manhattan;
using Encog.Engine.Network.Flat;
using Encog.Engine.Network.Train;
using Encog.Engine.Network.Train.Prop;

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Implements basic functionality that is needed by each of the propagation
    /// methods. The specifics of each of the propagation methods is implemented
    /// inside of the PropagationMethod interface implementors.
    /// </summary>
    public abstract class Propagation : BasicTraining
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(Propagation));
#endif

        /**
	 * The network.
	 */
        private BasicNetwork network;

        /**
         * The current flat network we are using for training, or null for none.
         */
        private FlatNetwork currentFlatNetwork;

        /**
         * The current flat trainer we are using, or null for none.
         */
        private ITrainFlatNetwork flatTraining;

        /**
         * Construct a propagation object.
         * 
         * @param network
         *            The network.
         * @param training
         *            The training set.
         */
        public Propagation(BasicNetwork network, INeuralDataSet training)
            : base()
        {
            this.network = network;
            Training = training;
        }

        /**
         * @return True if this training can be continued.
         */
        public virtual bool CanContinue
        {
            get
            {
                return false;
            }
        }

        /**
         * @return the currentFlatNetwork
         */
        public FlatNetwork CurrentFlatNetwork
        {
            get
            {
                return this.currentFlatNetwork;
            }
        }

        /**
         * @return the flatTraining
         */
        public ITrainFlatNetwork FlatTraining
        {
            get
            {
                return this.flatTraining;
            }
            set
            {
                this.flatTraining = value;
            }
        }

        /**
         * @return The network.
         */
        public override BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /**
         * @return The number of threads.
         */
        public int NumThreads
        {
            get
            {
                return this.flatTraining.NumThreads;
            }
            set
            {
                this.flatTraining.NumThreads = value;
            }
        }

        /**
         * Determine if this specified training continuation object is valid for
         * this training method.
         * 
         * @param state
         *            The training continuation object to check.
         * @return True if the continuation object is valid.
         */
        public virtual bool IsValidResume(TrainingContinuation state)
        {
            return false;
        }

        /**
         * Perform one training iteration.
         */
        public override void Iteration()
        {
            try
            {
                PreIteration();

                this.flatTraining.Iteration();
                this.Error = this.flatTraining.Error;
                this.network.Structure.FlatUpdate = FlatUpdateNeeded.Unflatten;

                PostIteration();

                if (LOGGER.IsInfoEnabled)
                {
                    LOGGER.Info("Training iteration done, error: " + this.Error);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                EncogValidate.ValidateNetworkForTraining(this.network,
                        Training);
                throw new EncogError(ex);
            }
        }



        /**
         * Pause the training to continue later.
         * 
         * @return A training continuation object.
         */
        public virtual TrainingContinuation Pause()
        {
            throw new TrainingError("This training type does not support pause.");
        }


        /**
         * Resume training.
         * 
         * @param state
         *            The training continuation object to use to continue.
         */
        public virtual void Resume(TrainingContinuation state)
        {
            throw new TrainingError("This training type does not support resume.");
        }




        /**
         * Should be called after training has completed and the iteration method
         * will not be called any further.
         */
        public void finishTraining()
        {
            base.FinishTraining();
            this.network.Structure.UpdateFlatNetwork();
            this.flatTraining.FinishTraining();
        }

        /**
         * @return The OpenCL device to use, or null for the CPU.
         */
        public OpenCLTrainingProfile Profile
        {
            get
            {
                return null;
            }
        }


        /**
         * Perform the specified number of training iterations. This can be more efficient than single 
         * training iterations.  This is particularly true if you are training with a GPU.
         * @param count The number of training iterations.
         */
        public void iteration(int count)
        {
            try
            {
                PreIteration();

                this.flatTraining.Iteration(count);
                this.CurrentIteration = this.flatTraining.CurrentIteration;
                this.Error = this.flatTraining.Error;
                this.network.Structure.FlatUpdate = FlatUpdateNeeded.Unflatten;

                PostIteration();

                if (LOGGER.IsInfoEnabled)
                {
                    LOGGER.Info("Training iterations done, error: " + this.Error);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                EncogValidate.ValidateNetworkForTraining(this.network,
                        Training);
                throw new EncogError(ex);
            }
        }

    }
}
