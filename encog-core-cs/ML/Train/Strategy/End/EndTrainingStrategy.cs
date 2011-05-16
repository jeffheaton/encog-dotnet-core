namespace Encog.ML.Train.Strategy.End
{
    public interface EndTrainingStrategy : IStrategy
    {
        /// <returns>True if training should stop.</returns>
        bool ShouldStop();
    }
}