using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Encog.ML.Train;
using Encog.Util;

namespace Encog.App.Analyst
{
    /// <summary>
    /// A console implementation of the Encog Analyst listener. Will report all
    /// progress to the console.
    /// </summary>
    ///
    public class ConsoleAnalystListener : AnalystListener
    {
        /// <summary>
        /// Stopwatch to time process.
        /// </summary>
        ///
        private readonly Stopwatch stopwatch;

        /// <summary>
        /// True if the current command should be canceled.
        /// </summary>
        ///
        private bool cancelCommand;

        /// <summary>
        /// The current task.
        /// </summary>
        ///
        private String currentTask;

        /// <summary>
        /// True if shutdown has been requested.
        /// </summary>
        ///
        private bool shutdownRequested;

        public ConsoleAnalystListener()
        {
            currentTask = "";
            stopwatch = new Stopwatch();
        }

        #region AnalystListener Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Report(int total, int current,
                           String message)
        {
            if (total == 0)
            {
                Console.Out.WriteLine(current + " : " + message);
            }
            else
            {
                Console.Out.WriteLine(current + "/" + total + " : " + message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void ReportCommandBegin(int total, int current,
                                       String name)
        {
            Console.Out.WriteLine();
            if (total == 0)
            {
                Console.Out.WriteLine("Beginning Task#" + current + " : " + name);
            }
            else
            {
                Console.Out.WriteLine("Beginning Task#" + current + "/" + total
                                      + " : " + name);
            }
            currentTask = name;
            stopwatch.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void ReportCommandEnd(bool cancel)
        {
            String cancelStr = "";
            cancelCommand = false;
            stopwatch.Stop();

            if (cancel)
            {
                cancelStr = "canceled";
            }
            else
            {
                cancelStr = "completed";
            }

            Console.Out.WriteLine("Task "
                                  + currentTask
                                  + " "
                                  + cancelStr
                                  + ", task elapsed time "
                                  + Format.FormatTimeSpan((int) (stopwatch.ElapsedMilliseconds/Format.MILI_IN_SEC)));
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void ReportTraining(MLTrain train)
        {
            Console.Out.WriteLine("Iteration #"
                                  + Format.FormatInteger(train.IterationNumber)
                                  + " Error:"
                                  + Format.FormatPercent(train.Error)
                                  + " elapsed time = "
                                  + Format.FormatTimeSpan((int) (stopwatch.ElapsedMilliseconds/Format.MILI_IN_SEC)));
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void ReportTrainingBegin()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void ReportTrainingEnd()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RequestCancelCommand()
        {
            cancelCommand = true;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RequestShutdown()
        {
            shutdownRequested = true;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool ShouldShutDown()
        {
            return shutdownRequested;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool ShouldStopCommand()
        {
            return cancelCommand;
        }

        #endregion
    }
}