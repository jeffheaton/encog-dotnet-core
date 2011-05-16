using Encog.MathUtil.Error;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.Neural.Networks.Training.Simple
{
    /// <summary>
    /// Train an ADALINE neural network.
    /// </summary>
    ///
    public class TrainAdaline : BasicTraining, ILearningRate
    {
        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly BasicNetwork network;

        /// <summary>
        /// The training data to use.
        /// </summary>
        ///
        private readonly MLDataSet training;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double learningRate;

        /// <summary>
        /// Construct an ADALINE trainer.
        /// </summary>
        ///
        /// <param name="network_0">The network to train.</param>
        /// <param name="training_1">The training data.</param>
        /// <param name="learningRate_2">The learning rate.</param>
        public TrainAdaline(BasicNetwork network_0, MLDataSet training_1,
                            double learningRate_2) : base(TrainingImplementationType.Iterative)
        {
            if (network_0.LayerCount > 2)
            {
                throw new NeuralNetworkError(
                    "An ADALINE network only has two layers.");
            }
            network = network_0;

            training = training_1;
            learningRate = learningRate_2;
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
        /// Set the learning rate.
        /// </summary>
        ///
        /// <value>The new learning rate.</value>
        public double LearningRate
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return learningRate; }
            /// <summary>
            /// Set the learning rate.
            /// </summary>
            ///
            /// <param name="rate">The new learning rate.</param>
            set { learningRate = value; }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            var errorCalculation = new ErrorCalculation();


            foreach (MLDataPair pair  in  training)
            {
                // calculate the error
                MLData output = network.Compute(pair.Input);

                for (int currentAdaline = 0; currentAdaline < output.Count; currentAdaline++)
                {
                    double diff = pair.Ideal[currentAdaline]
                                  - output[currentAdaline];

                    // weights
                    for (int i = 0; i <= network.InputCount; i++)
                    {
                        double input;

                        if (i == network.InputCount)
                        {
                            input = 1.0d;
                        }
                        else
                        {
                            input = pair.Input[i];
                        }

                        network.AddWeight(0, i, currentAdaline,
                                          learningRate*diff*input);
                    }
                }

                errorCalculation.UpdateError(output.Data, pair.Ideal.Data);
            }

            // set the global error
            Error = errorCalculation.Calculate();
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
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}