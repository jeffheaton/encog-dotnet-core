using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.EA.Exceptions
{
    /// <summary>
    /// The genome has generated a compile error and is invalid.
    /// </summary>
    public class EACompileError : EAError
    {
        /// <summary>
        /// Construct an error.
        /// </summary>
        /// <param name="msg">The error.</param>
        public EACompileError(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Construct the error.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="t">The error.</param>
        public EACompileError(String msg, Exception t)
            : base(msg, t)
        {
        }

        /// <summary>
        /// Construct the error.
        /// </summary>
        /// <param name="t">An error</param>
        public EACompileError(Exception t)
            : base(t)
        {
        }
    }
}
