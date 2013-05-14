using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Analyst;

namespace Encog.App.Generate
{
    /// <summary>
    /// Analyst code generation error.
    /// </summary>
    public class AnalystCodeGenerationError: AnalystError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        ///
        /// <param name="msg">The exception message.</param>
        public AnalystCodeGenerationError(String msg) : base(msg)
        {
        }

        /// <summary>
        /// Construct an exception that holds another exception.
        /// </summary>
        ///
        /// <param name="t">The other exception.</param>
        public AnalystCodeGenerationError(Exception t)
            : base(t)
        {
        }
    }
}
