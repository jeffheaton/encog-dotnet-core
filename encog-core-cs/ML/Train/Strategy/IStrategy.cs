namespace Encog.ML.Train.Strategy
{
    /// <summary>
    /// Training strategies can be added to training algorithms.  Training 
    /// strategies allow different additional logic to be added to an existing
    /// training algorithm.  There are a number of different training strategies
    /// that can perform various tasks, such as adjusting the learning rate or 
    /// momentum, or terminating training when improvement diminishes.  Other 
    /// strategies are provided as well.
    /// </summary>
    ///
    public interface IStrategy
    {
        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train">The training algorithm.</param>
        void Init(MLTrain train);

        /// <summary>
        /// Called just before a training iteration.
        /// </summary>
        ///
        void PreIteration();

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        ///
        void PostIteration();
    }
}