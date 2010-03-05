using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Encog.Util.Concurrency
{
    /// <summary>
    /// Defines a group of tasks that should finish together.
    /// </summary>
    public class TaskGroup
    {
        /// <summary>
        /// The ID for this task.
        /// </summary>
        private int id;

        /// <summary>
        /// The current number of tasks running in this group.
        /// </summary>
        private int currentTasks;

        /// <summary>
        /// Has this task group started.
        /// </summary>
        private bool started;

        /// <summary>
        /// Construct a task group.
        /// </summary>
        /// <param name="id">The ID of this task group.</param>
        public TaskGroup(int id)
        {
            this.id = id;
            this.currentTasks = 0;
            this.started = false;
        }

        /// <summary>
        /// The ID of this task group.
        /// </summary>
        public int ID
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// Notify this group that a task is starting up.
        /// </summary>
        public void TaskStarting()
        {
            lock (this)
            {
                this.currentTasks++;
                this.started = true;
            }
        }

        /// <summary>
        /// Notify this group that a task is shutting down.
        /// </summary>
        public void TaskStopping()
        {
            lock (this)
            {
                this.currentTasks--;
            }
        }

        /// <summary>
        /// Wait for this task group to start up.
        /// </summary>
        public void WaitForStart()
        {
            while (!started)
            {
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Wait for this task group to complete.
        /// </summary>
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
