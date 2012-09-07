using System;

namespace Encog.Cloud
{
    /// <summary>
    /// An error has occured in one of the Encog Cloud classes.
    /// </summary>
    public class CloudError : EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public CloudError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public CloudError(Exception e)
            : base("Nested Exception", e)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="e">The exception.</param>
        public CloudError(String msg, Exception e)
            : base(msg, e)
        {
        }
    }
}