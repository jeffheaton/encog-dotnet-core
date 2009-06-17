using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Concurrency.Job
{
    /// <summary>
    /// A concurrent JOB, works well for multicore machines.  Provides
    /// the low-level tools to create a concurrent job.
    /// </summary>
    public abstract class ConcurrentJob
    {
        /// <summary>
        /// Where to report progress to.
        /// </summary>
        private StatusReportable report;

        /// <summary>
        /// Total number of tasks.
        /// </summary>
        private int totalTasks;

        /// <summary>
        /// Called by a thread to get the next task.
        /// </summary>
        /// <returns>Config info for the next task.</returns>
        public abstract Object RequestNextTask();

        /// <summary>
        /// Load the workload that this job must process.
        /// </summary>
        /// <returns></returns>
        public abstract int LoadWorkload();

        /// <summary>
        /// Perform the actual workload.
        /// </summary>
        /// <param name="context">The workload to execute.</param>
        public abstract void PerformJobUnit(JobUnitContext context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        public ConcurrentJob(StatusReportable report)
        {
            this.report = report;
        }

        /// <summary>
        /// Start the job, block until its done.
        /// </summary>
        public void Start()
        {
            Object task;

            this.totalTasks = LoadWorkload();
            int currentTask = 0;

            while ((task = RequestNextTask()) != null)
            {
                currentTask++;
                JobUnitContext context = new JobUnitContext();
                context.JobUnit = task;
                context.Owner = this;
                context.TaskNumber = currentTask;

                JobUnitWorker worker = new JobUnitWorker(context);
                EncogConcurrency.Instance.ProcessTask(worker);
            }

            EncogConcurrency.Instance.Shutdown(long.MaxValue);
        }

        /// <summary>
        /// Recieve status reports.
        /// </summary>
        /// <param name="context">The context for this job.</param>
        /// <param name="status">The current status for this job.</param>
        public void ReportStatus(JobUnitContext context, String status)
        {
            this.report.Report(totalTasks, context.TaskNumber, status);
        }
    }
}
