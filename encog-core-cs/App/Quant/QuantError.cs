using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant
{
    /// <summary>
    /// An error has occured in one of the quant classes.
    /// </summary>
    public class QuantError : EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public QuantError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public QuantError(Exception e)
            : base("Nested Exception", e)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="e">The exception.</param>
        public QuantError(String msg, Exception e)
            : base(msg, e)
        {
        }
    }
}
