using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.CL
{
    /// <summary>
    /// An error has occured while processing OpenCL.
    /// </summary>
    public class EncogCLError: EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public EncogCLError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public EncogCLError(Exception e)
            : base("Nested Exception", e)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="e">The exception.</param>
        public EncogCLError(String msg, Exception e)
            : base(msg,e)
        {
        }

    }
}
