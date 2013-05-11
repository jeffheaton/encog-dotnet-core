using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Exceptions;

namespace Encog.ML.Prg.ExpValue
{
    /// <summary>
    /// A division by zero.
    /// </summary>
    public class DivisionByZeroError : EARuntimeError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public DivisionByZeroError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public DivisionByZeroError(Exception e)
            : base("Nested Exception", e)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="e">The exception.</param>
        public DivisionByZeroError(String msg, Exception e)
            : base(msg, e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public DivisionByZeroError() : base("")
        {
        }
 
    }
}
