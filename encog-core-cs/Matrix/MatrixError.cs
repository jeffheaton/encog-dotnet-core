using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Matrix
{
    /// <summary>
    /// Indicates an error has occurred in Matrix classes..
    /// </summary>
    public class MatrixError : EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public MatrixError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public MatrixError(Exception e)
            : base(e)
        {
        }
    }
}
