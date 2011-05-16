namespace Encog.ML.Train.Strategy.End
{
    public class EndIterationsStrategy : EndTrainingStrategy
    {
        private readonly int maxIterations;
        private int currentIteration;
        private MLTrain train;

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