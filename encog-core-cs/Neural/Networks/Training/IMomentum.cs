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
        ///
        /// <value>The new momentum.</value>
        double Momentum { /// <returns>The momentum.</returns>
            get;
            /// <summary>
            /// Set the momentum.
            /// </summary>
            ///
            /// <param name="m">The new momentum.</param>
            set; }
    }
}