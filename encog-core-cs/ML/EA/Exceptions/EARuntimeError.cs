using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.EA.Exceptions
{
    /// <summary>
    /// An error has occurred while running a phenotype (or genome).
    /// </summary>
    public class EARuntimeError: EAError
    {
        /// <summary>
        /// Construct an error.
        /// </summary>
        /// <param name="msg">The error.</param>
        public EARuntimeError(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Construct the error.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="t">The error.</param>
        public EARuntimeError(String msg, Exception t)
            : base(msg, t)
        {
        }

        /// <summary>
        /// Construct the error.
        /// </summary>
        /// <param name="t">An error</param>
        public EARuntimeError(Exception t)
            : base(t)
        {
        }
    }
}
