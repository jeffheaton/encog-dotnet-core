namespace Encog.ML
{
    /// <summary>
    /// Defines a MLMethod that accepts input.  Input is defined as a simple 
    /// array of double values.  Many machine learning methods, such as neural 
    /// networks and support vector machines receive input this way, and thus 
    /// implement this interface.  Others, such as clustering, do not.
    /// </summary>
    ///
    public interface MLInput : MLMethod
    {
        /// <value>The input.</value>
        int InputCount { /// <returns>The input.</returns>
            get; }
    }
}