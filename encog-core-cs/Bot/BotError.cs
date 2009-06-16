using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Bot
{
    /// <summary>
    /// Indicates an error has occurred in the bot classes.
    /// </summary>
    public class BotError : EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public BotError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public BotError(Exception e)
            : base(e)
        {
        }
    }
}
