namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// Specifies that a training algorithm has the concept of a momentum.
    /// This allows it to be used with strategies that automatically adjust the
    /// momentum.
    /// </summary>
    ///
    public interface IMomentum
    {
        /// <summary>
        /// Set the momentum.
        /// </summary>
        double Momentum { 
            get;
            set; }
    }
}