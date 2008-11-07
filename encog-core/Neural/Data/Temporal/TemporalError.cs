using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Data.Temporal
{
    public class TemporalError : EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public TemporalError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public TemporalError(Exception e)
            : base(e)
        {
        }
    }
}
