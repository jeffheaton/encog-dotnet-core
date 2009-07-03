using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog
{
    /// <summary>
    /// Used to report status updates for some Encog tasks.
    /// </summary>
    public interface IStatusReportable
    {
        /// <summary>
        /// Called when an Encog job changes status.
        /// </summary>
        /// <param name="total">Total amount of work to be done.</param>
        /// <param name="current">Work currently being processed.</param>
        /// <param name="message">The current message.</param>
        void Report(int total, int current, String message);
    }
}
