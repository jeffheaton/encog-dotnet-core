using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Bot.Browse
{
    /// <summary>
    /// Indicates an error has occurred in the browse classes.
    /// </summary>
    public class BrowseError : EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public BrowseError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public BrowseError(Exception e)
            : base(e)
        {
        }
    }
}
