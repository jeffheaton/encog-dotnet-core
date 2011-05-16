namespace Encog.ML
{
    /// <summary>
    /// Defines a MLMethod that can hold context.  This allows the context to be 
    /// cleared.  Examples of MLMethod objects that support this are NEAT, 
    /// Elmann and Jordan.
    /// </summary>
    ///
    public interface MLContext : MLMethod
    {
        /// <summary>
        /// Clear the context.
        /// </summary>
        ///
        void ClearContext();
    }
}