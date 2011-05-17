namespace Encog.ML.Train.Strategy.End
{
    /// <summary>
    /// A training strategy that specifies when to end training.
    /// </summary>
    public interface EndTrainingStrategy : IStrategy
    {
        /// <returns>True if training should stop.</returns>
        bool ShouldStop();
    }
}