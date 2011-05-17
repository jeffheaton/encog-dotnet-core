using System;
using Encog.ML.Train;

namespace Encog.App.Analyst
{
    /// <summary>
    /// Reports the progress of the Encog Analyst. If you would like to use this with
    /// an Encog StatusReportable object, use the bridge utilituy object:
    /// Encog.app.analyst.util.AnalystReportBridge
    /// </summary>
    ///
    public interface AnalystListener
    {
        /// <summary>
        /// Request stop the entire process.
        /// </summary>
        ///
        void RequestShutdown();

        /// <summary>
        /// Request to cancel current command.
        /// </summary>
        ///
        void RequestCancelCommand();


        /// <returns>True if the entire process should be stopped.</returns>
        bool ShouldShutDown();


        /// <returns>True if the current command should be stopped.</returns>
        bool ShouldStopCommand();

        /// <summary>
        /// Report that a command has begun.
        /// </summary>
        ///
        /// <param name="total">The total parts.</param>
        /// <param name="current">The current part.</param>
        /// <param name="name">The name of that command.</param>
        void ReportCommandBegin(int total, int current, String name);

        /// <summary>
        /// Report that a command has ended.
        /// </summary>
        ///
        /// <param name="canceled">True if this command was canceled.</param>
        void ReportCommandEnd(bool canceled);

        /// <summary>
        /// Report that training has begun.
        /// </summary>
        ///
        void ReportTrainingBegin();

        /// <summary>
        /// Report that training has ended.
        /// </summary>
        ///
        void ReportTrainingEnd();

        /// <summary>
        /// Report progress on training.
        /// </summary>
        ///
        /// <param name="train">The training object.</param>
        void ReportTraining(MLTrain train);

        /// <summary>
        /// Report progress on a task.
        /// </summary>
        ///
        /// <param name="total">The total number of commands.</param>
        /// <param name="current">The current command.</param>
        /// <param name="message">The message.</param>
        void Report(int total, int current, String message);
    }
}