// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Engine.Concurrency.Job
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
        private IStatusReportable report;

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
        public ConcurrentJob(IStatusReportable report)
        {
            this.report = report;
        }

        /// <summary>
        /// Start the job, block until its done.
        /// </summary>
        public virtual void Process()
        {
            Object task;

            TaskGroup group = EncogConcurrency.Instance.CreateTaskGroup();

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
                EncogConcurrency.Instance.ProcessTask(worker, group);
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
            this.report.Report(totalTasks, context.TaskNumber, status);
        }
    }
}
