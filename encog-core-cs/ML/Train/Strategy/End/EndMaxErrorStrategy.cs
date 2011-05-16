namespace Encog.ML.Train.Strategy.End
{
    public class EndMaxErrorStrategy : EndTrainingStrategy
    {
        private readonly double maxError;
        private bool started;
        private MLTrain train;

        public EndMaxErrorStrategy(double maxError_0)
        {
            maxError = maxError_0;
            started = false;
        }

        #region EndTrainingStrategy Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual bool ShouldStop()
        {
            return started && train.Error < maxError;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void Init(MLTrain train_0)
        {
            train = train_0;
            started = false;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PostIteration()
        {
            started = true;
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