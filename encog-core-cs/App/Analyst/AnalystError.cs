using System;

namespace Encog.App.Analyst
{
    /// <summary>
    /// An error has occured with the Encog Analyst.
    /// </summary>
    ///
    [Serializable]
    public class AnalystError : EncogError
    {

        /// <summary>
        /// Construct a message exception.
        /// </summary>
        ///
        /// <param name="msg">The exception message.</param>
        public AnalystError(String msg) : base(msg)
        {
        }

        /// <summary>
        /// Construct an exception that holds another exception.
        /// </summary>
        ///
        /// <param name="t">The other exception.</param>
        public AnalystError(Exception t) : base(t)
        {
        }
    }
}