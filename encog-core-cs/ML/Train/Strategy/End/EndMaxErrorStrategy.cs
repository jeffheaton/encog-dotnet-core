namespace Encog.ML.Train.Strategy.End
{
    /// <summary>
    /// End training once the error falls below a specified level.
    /// </summary>
    public class EndMaxErrorStrategy : EndTrainingStrategy
    {
        /// <summary>
        /// The max error.
        /// </summary>
        private readonly double maxError;

        /// <summary>
        /// Has training started.
        /// </summary>
        private bool started;

        /// <summary>
        /// The trainer.
        /// </summary>
        private MLTrain train;

        /// <summary>
        /// Construct the object, specify the max error.
        /// </summary>
        /// <param name="maxError_0">The max error.</param>
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