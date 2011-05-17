namespace Encog.App.Quant
{
    /// <summary>
    /// Defines an interface for Encog quant tasks.
    /// </summary>
    ///
    public interface QuantTask
    {
        /// <summary>
        /// Request to stop.
        /// </summary>
        ///
        void RequestStop();


        /// <returns>Determine if we should stop.</returns>
        bool ShouldStop();
    }
}