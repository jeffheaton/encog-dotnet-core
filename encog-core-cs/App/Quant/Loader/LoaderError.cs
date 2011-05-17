using System;

namespace Encog.App.Quant.Loader
{
    /// <summary>
    /// Used to represent any error that occurs in the loader part of Encog.
    /// </summary>
    ///
    [Serializable]
    public class LoaderError : QuantError
    {
        /// <summary>
        /// The serial id.
        /// </summary>
        ///
        private const long serialVersionUID = 7737393439642876303L;

        /// <summary>
        /// Construct a message exception.
        /// </summary>
        ///
        /// <param name="msg">The exception message.</param>
        public LoaderError(String msg) : base(msg)
        {
        }

        /// <summary>
        /// Construct an exception that holds another exception.
        /// </summary>
        ///
        /// <param name="t">The other exception.</param>
        public LoaderError(Exception t) : base(t)
        {
        }
    }
}