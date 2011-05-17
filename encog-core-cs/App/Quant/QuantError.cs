using System;

namespace Encog.App.Quant
{
    /// <summary>
    /// Used to represent any error that occurs in the quant part of Encog.
    /// </summary>
    ///
    [Serializable]
    public class QuantError : EncogError
    {
        private const long serialVersionUID = 4280940104791165511L;

        /// <summary>
        /// Construct a message exception.
        /// </summary>
        ///
        /// <param name="msg">The exception message.</param>
        public QuantError(String msg) : base(msg)
        {
        }

        /// <summary>
        /// Construct an exception that holds another exception.
        /// </summary>
        ///
        /// <param name="t">The other exception.</param>
        public QuantError(Exception t) : base(t)
        {
        }
    }
}