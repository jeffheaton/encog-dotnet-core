using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Util.Logging;

namespace Encog.Neural.Networks.Training.Strategy
{
    /// <summary>
    /// Attempt to automatically set the learning rate in a learning method that
    /// supports a learning rate.
    /// </summary>
    ///
    public class SmartLearningRate : IStrategy
    {
        /// <summary>
        /// Learning decay rate.
        /// </summary>
        ///
        public const double LEARNING_DECAY = 0.99d;

        /// <summary>
        /// The current learning rate.
        /// </summary>
        ///
        private double currentLearningRate;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double lastError;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start evaluation.
        /// </summary>
        ///
        private bool ready;

        /// <summary>
        /// The class that is to have the learning rate set for.
        /// </summary>
        ///
        private ILearningRate setter;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private MLTrain train;

        /// <summary>
        /// The training set size, this is used to pick an initial learning rate.
        /// </summary>
        ///
        private long trainingSize;

        #region IStrategy Members

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train_0">The training algorithm.</param>
        public void Init(MLTrain train_0)
        {
            train = train_0;
            ready = false;
            setter = (ILearningRate) train_0;
            trainingSize = train_0.Training.Count;
            currentLearningRate = 1.0d/trainingSize;
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "Starting learning rate: "
                                                       + currentLearningRate);
            setter.LearningRate = currentLearningRate;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        ///
        public void PostIteration()
        {
            if (ready)
            {
                if (train.Error > lastError)
                {
                    currentLearningRate *= LEARNING_DECAY;
                    setter.LearningRate = currentLearningRate;
                    EncogLogging.Log(EncogLogging.LEVEL_DEBUG,
                                     "Adjusting learning rate to {}"
                                     + currentLearningRate);
                }
            }
            else
            {
                ready = true;
            }
        }

        /// <summary>
        /// Called just before a training iteration.
        /// </summary>
        ///
        public void PreIteration()
        {
            lastError = train.Error;
        }

        #endregion
    }
}