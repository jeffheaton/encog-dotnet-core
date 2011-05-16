using System;

namespace Encog
{
    /// <summary>
    /// This class allows for Encog jobs to report their current status, as they run.
    /// </summary>
    ///
    public interface IStatusReportable
    {
        /// <summary>
        /// Report on current status.
        /// </summary>
        ///
        /// <param name="total">The total amount of units to process.</param>
        /// <param name="current">The current unit being processed.</param>
        /// <param name="message">The message to currently display.</param>
        void Report(int total, int current, String message);
    }
}