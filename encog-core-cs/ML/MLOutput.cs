namespace Encog.ML
{
    /// <summary>
    /// Defines a MLMethod that produces output.  Input is defined as a simple 
    /// array of double values.  Many machine learning methods, such as neural 
    /// networks and support vector machines produce output this way, and thus 
    /// implement this interface.  Others, such as clustering, do not.
    /// </summary>
    ///
    public interface MLOutput : MLMethod
    {
        /// <value>The output count.</value>
        int OutputCount { get; }
    }
}