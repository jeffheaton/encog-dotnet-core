using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Encog.Util.Concurrency
{
    /// <summary>
    /// A task group is a group of tasks that you would like to execute at once.  
    /// You can wait for all tasks in a task group to exit before your program
    /// continues.
    /// </summary>
    public class TaskGroup
    {
        /// <summary>
        /// The ID for this task group.
        /// </summary>
        private int id;

        /// <summary>
        /// The total number of tasks in this group.
        /// </summary>
        private int totalTasks;

        /// <summary>
        /// The number of tasks that have completed.
        /// </summary>
        private int completedTasks;

        /// <summary>
        /// The event used to sync waiting for tasks to stop.
        /// </summary>
        private ManualResetEvent completeEvent = new ManualResetEvent(false);


        /// <summary>
        /// Create a task group with the specified id.
        /// </summary>
        /// <param name="id">The ID of the task group.</param>
        public TaskGroup(int id)
        {
            this.id = id;
            this.totalTasks = 0;
        }

        /// <summary>
        /// The ID of the task group.
        /// </summary>
        public int ID
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// Notify that a task is starting.
        /// </summary>
        public void TaskStarting()
        {
            lock (this)
            {
                this.totalTasks++;
            }
        }
        
        /// <summary>
        /// Notify that a task is stopping.
        /// </summary>
        public void TaskStopping()
        {
            lock (this)
            {
                this.completedTasks++;
                this.completeEvent.Set();
            }
        }

        /// <summary>
        /// Returns true if there are no more tasks.
        /// </summary>
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

        /// <summary>
        /// Wait for all tasks to complete in this group.
        /// </summary>
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
