using System;

namespace Encog.Cloud.Indicator
{
    /// <summary>
    /// An error has occured communicating with an external indicator.
    /// </summary>
    public class IndicatorError : CloudError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public IndicatorError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public IndicatorError(Exception e)
            : base("Nested Exception", e)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="e">The exception.</param>
        public IndicatorError(String msg, Exception e)
            : base(msg, e)
        {
        }
    }
}