//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using Encog.ML.Train;

namespace Encog.App.Analyst
{
    /// <summary>
    ///     Reports the progress of the Encog Analyst. If you would like to use this with
    ///     an Encog StatusReportable object, use the bridge utilituy object:
    ///     Encog.app.analyst.util.AnalystReportBridge
    /// </summary>
    public interface IAnalystListener
    {
        /// <summary>
        ///     Request stop the entire process.
        /// </summary>
        void RequestShutdown();

        /// <summary>
        ///     Request to cancel current command.
        /// </summary>
        void RequestCancelCommand();


        /// <returns>True if the entire process should be stopped.</returns>
        bool ShouldShutDown();


        /// <returns>True if the current command should be stopped.</returns>
        bool ShouldStopCommand();

        /// <summary>
        ///     Report that a command has begun.
        /// </summary>
        /// <param name="total">The total parts.</param>
        /// <param name="current">The current part.</param>
        /// <param name="name">The name of that command.</param>
        void ReportCommandBegin(int total, int current, String name);

        /// <summary>
        ///     Report that a command has ended.
        /// </summary>
        /// <param name="canceled">True if this command was canceled.</param>
        void ReportCommandEnd(bool canceled);

        /// <summary>
        ///     Report that training has begun.
        /// </summary>
        void ReportTrainingBegin();

        /// <summary>
        ///     Report that training has ended.
        /// </summary>
        void ReportTrainingEnd();

        /// <summary>
        ///     Report progress on training.
        /// </summary>
        /// <param name="train">The training object.</param>
        void ReportTraining(IMLTrain train);

        /// <summary>
        ///     Report progress on a task.
        /// </summary>
        /// <param name="total">The total number of commands.</param>
        /// <param name="current">The current command.</param>
        /// <param name="message">The message.</param>
        void Report(int total, int current, String message);
    }
}
