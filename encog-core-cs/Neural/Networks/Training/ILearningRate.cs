namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// Specifies that a training algorithm has the concept of a learning rate.
    /// This allows it to be used with strategies that automatically adjust the
    /// learning rate.
    /// </summary>
    ///
    public interface ILearningRate
    {
        /// <summary>
        /// Set the learning rate.
        /// </summary>
        double LearningRate { 
            get;
            set; }
    }
}