//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
        private readonly IStatusReportable report;

        /// <summary>
        /// Total number of tasks.
        /// </summary>
        private int totalTasks;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        public ConcurrentJob(IStatusReportable report)
        {
            this.report = report;
        }

        /// <summary>
        /// Has a stop been requested?
        /// </summary>
        public bool ShouldStop { get; set; }

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
        /// Start the job, block until its done.
        /// </summary>
        public virtual void Process()
        {
            Object task;

            TaskGroup group = EngineConcurrency.Instance.CreateTaskGroup();

            totalTasks = LoadWorkload();
            int currentTask = 0;

            while ((task = RequestNextTask()) != null)
            {
                currentTask++;
                var context = new JobUnitContext();
                context.JobUnit = task;
                context.Owner = this;
                context.TaskNumber = currentTask;

                var worker = new JobUnitWorker(context);
                EngineConcurrency.Instance.ProcessTask(worker, group);
            }

            group.WaitForComplete();
        }

        /// <summary>
        /// Recieve status reports.
        /// </summary>
        /// <param name="context">The context for this job.</param>
        /// <param name="status">The current status for this job.</param>
        public void ReportStatus(JobUnitContext context, String status)
        {
            report.Report(totalTasks, context.TaskNumber, status);
        }
    }
}
