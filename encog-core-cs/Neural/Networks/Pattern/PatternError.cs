using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Pattern
{
    /// <summary>
    /// Indicates an error has occurred in the pattern classes.
    /// </summary>
    public class PatternError : EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public PatternError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public PatternError(Exception e)
            : base(e)
        {
        }
    }
}
