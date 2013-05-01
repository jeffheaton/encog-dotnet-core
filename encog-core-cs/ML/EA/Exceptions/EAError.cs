using System;
namespace Encog.ML.EA.Exceptions
{
    /// <summary>
    /// A general evolutionary algorithm error.
    /// </summary>
    public class EAError : EncogError
    {
        /// <summary>
        /// Construct the exception.  Errors can be further divided into runtime or compile.
        /// </summary>
        /// <param name="msg">The message.</param>
        public EAError(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Construct the exception.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="t">A throwable error.</param>
        public EAError(string msg, Exception t)
            : base(msg, t)
        {
        }

        /// <summary>
        /// Construct the exception.
        /// </summary>
        /// <param name="t">A throwable error.</param>
        public EAError(Exception t)
            : base(t)
        {
        }
    }
}
