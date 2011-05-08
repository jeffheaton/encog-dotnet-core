using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Script
{
    public class ScriptError: EncogError
    {
                        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public ScriptError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public ScriptError(Exception e)
            : base("Nested Exception", e)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="e">The exception.</param>
        public ScriptError(String msg, Exception e)
            : base(msg,e)
        {
        }

    }
}
