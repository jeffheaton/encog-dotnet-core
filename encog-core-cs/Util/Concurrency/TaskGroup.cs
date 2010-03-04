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
        private int currentTasks;
        private bool started;

        public TaskGroup(int id)
        {
            this.id = id;
            this.currentTasks = 0;
            this.started = false;
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
                this.currentTasks++;
                this.started = true;
            }
        }

        public void TaskStopping()
        {
            lock (this)
            {
                this.currentTasks--;
            }
        }

        public void WaitForStart()
        {
            while (!started)
            {
                Thread.Sleep(100);
            }
        }

        public void WaitForComplete()
        {
            WaitForStart();
            while (this.currentTasks > 0)
            {
                Thread.Sleep(100);
            }
        }
    }
}
