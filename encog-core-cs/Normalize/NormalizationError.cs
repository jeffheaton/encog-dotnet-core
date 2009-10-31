using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Normalize
{
    /// <summary>
    /// Used for normalization errors.
    /// </summary>
    public class NormalizationError : EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public NormalizationError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public NormalizationError(Exception e)
            : base(e)
        {
        }
    }
}
