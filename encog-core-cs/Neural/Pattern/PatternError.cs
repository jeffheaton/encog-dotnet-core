using System;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// This class is thrown when an error occurs while using one of the neural
    /// network pattern classes.
    /// </summary>
    ///
    [Serializable]
    public class PatternError : NeuralNetworkError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        ///
        /// <param name="msg">The exception message.</param>
        public PatternError(String msg) : base(msg)
        {
        }

        /// <summary>
        /// Construct an exception that holds another exception.
        /// </summary>
        ///
        /// <param name="t">The other exception.</param>
        public PatternError(Exception t) : base(t)
        {
        }
    }
}