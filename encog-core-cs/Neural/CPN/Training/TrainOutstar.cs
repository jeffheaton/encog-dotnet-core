using Encog.MathUtil.Error;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util;

namespace Encog.Neural.CPN.Training
{
    /// <summary>
    /// Used for Instar training of a CPN neural network. A CPN network is a hybrid
    /// supervised/unsupervised network. The Outstar training handles the supervised
    /// portion of the training.
    /// </summary>
    ///
    public class TrainOutstar : BasicTraining, ILearningRate
    {
        /// <summary>
        /// The network being trained.
        /// </summary>
        ///
        private readonly CPNNetwork network;

        /// <summary>
        /// The training data. Supervised training, so both input and ideal must be
        /// provided.
        /// </summary>
        ///
        private readonly MLDataSet training;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double learningRate;

        /// <summary>
        /// If the weights have not been initialized, then they must be initialized
        /// before training begins. This will be done on the first iteration.
        /// </summary>
        ///
        private bool mustInit;

        /// <summary>
        /// Construct the outstar trainer.
        /// </summary>
        ///
        /// <param name="theNetwork">The network to train.</param>
        /// <param name="theTraining">The training data, must provide ideal outputs.</param>
        /// <param name="theLearningRate">The learning rate.</param>
        public TrainOutstar(CPNNetwork theNetwork, MLDataSet theTraining,
                            double theLearningRate) : base(TrainingImplementationType.Iterative)
        {
            mustInit = true;
            network = theNetwork;
            training = theTraining;
            learningRate = theLearningRate;
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
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

        #region ILearningRate Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public double LearningRate
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return learningRate; }
            /// <summary>
            /// 
            /// </summary>
            ///
            set { learningRate = value; }
        }

        #endregion

        /// <summary>
        /// Approximate the weights based on the input values.
        /// </summary>
        ///
        private void InitWeight()
        {
            for (int i = 0; i < network.OutstarCount; i++)
            {
                int j = 0;

                foreach (MLDataPair pair  in  training)
                {
                    network.WeightsInstarToOutstar[j++, i] =
                        pair.Ideal[i];
                }
            }
            mustInit = false;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            if (mustInit)
            {
                InitWeight();
            }

            var error = new ErrorCalculation();


            foreach (MLDataPair pair  in  training)
            {
                MLData xout = network.ComputeInstar(pair.Input);

                int j = EngineArray.IndexOfLargest(xout.Data);
                for (int i = 0; i < network.OutstarCount; i++)
                {
                    double delta = learningRate
                                   *(pair.Ideal[i] - network.WeightsInstarToOutstar[j, i]);
                    network.WeightsInstarToOutstar.Add(j, i, delta);
                }

                MLData out2 = network.ComputeOutstar(xout);
                error.UpdateError(out2.Data, pair.Ideal.Data);
            }

            Error = error.Calculate();
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void Resume(TrainingContinuation state)
        {
        }
    }
}