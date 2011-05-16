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
        /// The event used to sync waiting for tasks to stop.
        /// </summary>
        private readonly ManualResetEvent completeEvent = new ManualResetEvent(false);

        /// <summary>
        /// The ID for this task group.
        /// </summary>
        private readonly int id;

        /// <summary>
        /// The number of tasks that have completed.
        /// </summary>
        private int completedTasks;

        /// <summary>
        /// The total number of tasks in this group.
        /// </summary>
        private int totalTasks;


        /// <summary>
        /// Create a task group with the specified id.
        /// </summary>
        /// <param name="id">The ID of the task group.</param>
        public TaskGroup(int id)
        {
            this.id = id;
            totalTasks = 0;
        }

        /// <summary>
        /// The ID of the task group.
        /// </summary>
        public int ID
        {
            get { return id; }
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
                    return totalTasks == completedTasks;
                }
            }
        }

        /// <summary>
        /// Notify that a task is starting.
        /// </summary>
        public void TaskStarting()
        {
            lock (this)
            {
                totalTasks++;
            }
        }

        /// <summary>
        /// Notify that a task is stopping.
        /// </summary>
        public void TaskStopping()
        {
            lock (this)
            {
                completedTasks++;
                completeEvent.Set();
            }
        }

        /// <summary>
        /// Wait for all tasks to complete in this group.
        /// </summary>
        public void WaitForComplete()
        {
            while (!NoTasks)
            {
                completeEvent.WaitOne();
                completeEvent.Reset();
            }
        }
    }
}