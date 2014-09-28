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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Encog.ML.Train;
using Encog.Util;

namespace Encog.App.Analyst
{
    /// <summary>
    ///     A console implementation of the Encog Analyst listener. Will report all
    ///     progress to the console.
    /// </summary>
    public class ConsoleAnalystListener : IAnalystListener
    {
        /// <summary>
        ///     Stopwatch to time process.
        /// </summary>
        private readonly Stopwatch _stopwatch;

        /// <summary>
        ///     True if the current command should be canceled.
        /// </summary>
        private bool _cancelCommand;

        /// <summary>
        ///     The current task.
        /// </summary>
        private String _currentTask;

        /// <summary>
        ///     True if shutdown has been requested.
        /// </summary>
        private bool _shutdownRequested;

        /// <summary>
        ///     Construct the object.
        /// </summary>
        public ConsoleAnalystListener()
        {
            _currentTask = "";
            _stopwatch = new Stopwatch();
        }

        #region AnalystListener Members

        /// <summary>
        /// </summary>
        public void Report(int total, int current,
                           String message)
        {
            string msg;
            if (total == 0)
            {
                msg = current + " : " + message;
            }
            else
            {
                msg = current + "/" + total + " : " + message;
            }
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
        }

        /// <summary>
        /// </summary>
        public void ReportCommandBegin(int total, int current,
                                       String name)
        {
            Console.WriteLine();
            string msg;
            if (total == 0)
            {
                msg = "Beginning Task#" + current + " : " + name;
            }
            else
            {
                msg = "Beginning Task#" + current + "/" + total
                      + " : " + name;
            }
            Debug.WriteLine(msg);
            Console.WriteLine(msg);
            _currentTask = name;
            _stopwatch.Start();
        }

        /// <summary>
        /// </summary>
        public void ReportCommandEnd(bool cancel)
        {
            String cancelStr = "";
            _cancelCommand = false;
            _stopwatch.Stop();

            cancelStr = cancel ? "canceled" : "completed";

            string msg = "Task "
                         + _currentTask
                         + " "
                         + cancelStr
                         + ", task elapsed time: "
                         + _stopwatch.Elapsed.TotalSeconds + "s";
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
        }

        /// <summary>
        /// </summary>
        public void ReportTraining(IMLTrain train)
        {
            string msg = "Iteration #"
                         + Format.FormatInteger(train.IterationNumber)
                         + " Error:"
                         + Format.FormatPercent(train.Error)
                         + " elapsed time = "
                         + Format.FormatTimeSpan((int) (_stopwatch.ElapsedMilliseconds/Format.MiliInSec));
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
        }

        /// <summary>
        /// </summary>
        public virtual void ReportTrainingBegin()
        {
        }

        /// <summary>
        /// </summary>
        public virtual void ReportTrainingEnd()
        {
        }

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RequestCancelCommand()
        {
            _cancelCommand = true;
        }

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RequestShutdown()
        {
            _shutdownRequested = true;
        }

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool ShouldShutDown()
        {
            return _shutdownRequested;
        }

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool ShouldStopCommand()
        {
            return _cancelCommand;
        }

        #endregion
    }
}
