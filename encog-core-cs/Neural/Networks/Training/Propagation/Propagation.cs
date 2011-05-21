//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Flat;
using Encog.Neural.Flat.Train;
using Encog.Util;
using Encog.Util.Logging;
using Encog.Neural.Flat.Train.Prop;

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Implements basic functionality that is needed by each of the propagation
    /// methods. The specifics of each of the propagation methods is implemented
    /// inside of the PropagationMethod interface implementors.
    /// </summary>
    ///
    public abstract class Propagation : BasicTraining, Train
    {
        /// <summary>
        /// The network.
        /// </summary>
        ///
        private readonly ContainsFlat network;

        /// <summary>
        /// The current flat trainer we are using, or null for none.
        /// </summary>
        ///
        private ITrainFlatNetwork flatTraining;

        /// <summary>
        /// Construct a propagation object.
        /// </summary>
        ///
        /// <param name="network_0">The network.</param>
        /// <param name="training">The training set.</param>
        public Propagation(ContainsFlat network_0, IMLDataSet training) : base(TrainingImplementationType.Iterative)
        {
            network = network_0;
            Training = training;
        }

        /// <value>the flatTraining to set</value>
        public ITrainFlatNetwork FlatTraining
        {
            get { return flatTraining; }
            set { flatTraining = value; }
        }


        /// <summary>
        /// Set the number of threads. Specify zero to tell Encog to automatically
        /// determine the best number of threads for the processor. If OpenCL is used
        /// as the target device, then this value is not used.
        /// </summary>
        public int NumThreads
        {
            get { return flatTraining.NumThreads; }
            set { flatTraining.NumThreads = value; }
        }


        /// <summary>
        /// Default is true.  Call this with false to disable flat spot fix.
        /// 
        /// For more info on flat spot:
        /// 
        /// http://www.heatonresearch.com/wiki/Flat_Spot
        /// 
        /// </summary>
        public bool FixFlatSpot 
        {
            get
            {
                return ((TrainFlatNetworkProp)this.flatTraining).FixFlatSpot;
            }
            set
            {
                ((TrainFlatNetworkProp)this.flatTraining).FixFlatSpot = value;
            }
        }

        #region Train Members

        /// <summary>
        /// Should be called after training has completed and the iteration method
        /// will not be called any further.
        /// </summary>
        ///
        public override sealed void FinishTraining()
        {
            base.FinishTraining();
            flatTraining.FinishTraining();
        }

        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return network; }
        }


        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            try
            {
                PreIteration();

                flatTraining.Iteration();
                Error = flatTraining.Error;

                PostIteration();

                EncogLogging.Log(EncogLogging.LEVEL_INFO,
                                 "Training iteration done, error: " + Error);
            }
            catch (IndexOutOfRangeException ex)
            {
                EncogValidate.ValidateNetworkForTraining(network,
                                                         Training);
                throw new EncogError(ex);
            }
        }

        /// <summary>
        /// Perform the specified number of training iterations. This can be more
        /// efficient than single training iterations. This is particularly true if
        /// you are training with a GPU.
        /// </summary>
        ///
        /// <param name="count">The number of training iterations.</param>
        public override sealed void Iteration(int count)
        {
            try
            {
                PreIteration();

                flatTraining.Iteration(count);
                IterationNumber = flatTraining.IterationNumber;
                Error = flatTraining.Error;

                PostIteration();

                EncogLogging.Log(EncogLogging.LEVEL_INFO,
                                 "Training iterations done, error: " + Error);
            }
            catch (IndexOutOfRangeException ex)
            {
                EncogValidate.ValidateNetworkForTraining(network,
                                                         Training);
                throw new EncogError(ex);
            }
        }

        #endregion
    }
}
