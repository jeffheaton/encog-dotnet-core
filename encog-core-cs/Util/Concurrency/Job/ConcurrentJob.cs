using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Concurrency.Job
{
    public abstract class ConcurrentJob
    {

        private StatusReportable report;
        private int totalTasks;

        public abstract Object RequestNextTask();
        public abstract int LoadWorkload();
        public abstract void PerformJobUnit(JobUnitContext context);


        public ConcurrentJob(StatusReportable report)
        {
            this.report = report;
        }

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

        public void ReportStatus(JobUnitContext context, String status)
        {
            this.report.Report(totalTasks, context.TaskNumber, status);
        }
    }
}
