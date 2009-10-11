using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.CSV
{
    /// <summary>
    /// Used to report errors for CSV formatting.
    /// </summary>
    public class CSVError: EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public CSVError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public CSVError(Exception e)
            : base(e)
        {
        }
    }
}
