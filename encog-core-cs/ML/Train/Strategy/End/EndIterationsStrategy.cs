namespace Encog.ML.Train.Strategy.End
{
    /// <summary>
    /// End the training when a specified number of iterations has been reached.
    /// </summary>
    public class EndIterationsStrategy : EndTrainingStrategy
    {
        private readonly int maxIterations;
        private int currentIteration;
        private MLTrain train;

        /// <summary>
        /// Construct the object, specify the max number of iterations.
        /// </summary>
        /// <param name="maxIterations_0">The number of iterations.</param>
        public EndIterationsStrategy(int maxIterations_0)
        {
            maxIterations = maxIterations_0;
            currentIteration = 0;
        }

        #region EndTrainingStrategy Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual bool ShouldStop()
        {
            return (currentIteration >= maxIterations);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void Init(MLTrain train_0)
        {
            train = train_0;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PostIteration()
        {
            currentIteration = train.IterationNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PreIteration()
        {
        }

        #endregion
    }
}