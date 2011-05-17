using System;

namespace Encog.Persist
{
    /// <summary>
    /// General error class for Encog persistence.
    /// </summary>
    ///
    [Serializable]
    public class PersistError : EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        ///
        /// <param name="msg">The exception message.</param>
        public PersistError(String msg) : base(msg)
        {
        }

        /// <summary>
        /// Construct an exception that holds another exception.
        /// </summary>
        ///
        /// <param name="msg">The message.</param>
        /// <param name="t">The other exception.</param>
        public PersistError(String msg, Exception t) : base(msg, t)
        {
        }

        /// <summary>
        /// Construct an exception that holds another exception.
        /// </summary>
        ///
        /// <param name="t">The other exception.</param>
        public PersistError(Exception t) : base(t)
        {
        }
    }
}