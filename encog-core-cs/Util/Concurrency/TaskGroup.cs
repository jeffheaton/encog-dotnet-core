using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Encog.Util.Concurrency
{
    public class TaskGroup
    {
        private int id;
        private int totalTasks;
        private int completedTasks;
        private ManualResetEvent completeEvent = new ManualResetEvent(false);

        public TaskGroup(int id)
        {
            this.id = id;
            this.totalTasks = 0;
        }

        public int ID
        {
            get
            {
                return this.id;
            }
        }

        public void TaskStarting()
        {
            lock (this)
            {
                this.totalTasks++;
            }
        }

        public void TaskStopping()
        {
            lock (this)
            {
                this.completedTasks++;
                this.completeEvent.Set();
            }
        }

        public bool NoTasks
        {
            get
            {
                lock (this)
                {
                    return this.totalTasks == this.completedTasks;
                }
            }
        }

        public void WaitForComplete()
        {
            while (!NoTasks)
            {
                this.completeEvent.WaitOne();
                this.completeEvent.Reset();
            }
        }
    }
}
