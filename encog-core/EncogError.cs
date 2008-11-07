using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog
{
    public class EncogError: Exception
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public EncogError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public EncogError(Exception e)
            : base("Nested Exception", e)
        {
        }
    }
}
