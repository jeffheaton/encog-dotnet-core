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
        private TrainFlatNetwork flatTraining;

        /// <summary>
        /// Construct a propagation object.
        /// </summary>
        ///
        /// <param name="network_0">The network.</param>
        /// <param name="training">The training set.</param>
        public Propagation(ContainsFlat network_0, MLDataSet training) : base(TrainingImplementationType.Iterative)
        {
            network = network_0;
            Training = training;
        }

        /// <value>the flatTraining to set</value>
        public TrainFlatNetwork FlatTraining
        {
            /// <returns>the flatTraining</returns>
            get { return flatTraining; }
            /// <param name="flatTraining_0">the flatTraining to set</param>
            set { flatTraining = value; }
        }


        /// <summary>
        /// Set the number of threads. Specify zero to tell Encog to automatically
        /// determine the best number of threads for the processor. If OpenCL is used
        /// as the target device, then this value is not used.
        /// </summary>
        ///
        /// <value>The number of threads.</value>
        public int NumThreads
        {
            /// <returns>The number of threads.</returns>
            get { return flatTraining.NumThreads; }
            /// <summary>
            /// Set the number of threads. Specify zero to tell Encog to automatically
            /// determine the best number of threads for the processor. If OpenCL is used
            /// as the target device, then this value is not used.
            /// </summary>
            ///
            /// <param name="numThreads">The number of threads.</param>
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

        /// <summary>
        /// 
        /// </summary>
        ///
        public override MLMethod Method
        {
            /// <summary>
            /// 
            /// </summary>
            ///
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