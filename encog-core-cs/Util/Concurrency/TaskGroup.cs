//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
        private readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);

        /// <summary>
        /// The ID for this task group.
        /// </summary>
        private readonly int _id;

        /// <summary>
        /// The number of tasks that have completed.
        /// </summary>
        private int _completedTasks;

        /// <summary>
        /// The total number of tasks in this group.
        /// </summary>
        private int _totalTasks;


        /// <summary>
        /// Create a task group with the specified id.
        /// </summary>
        /// <param name="id">The ID of the task group.</param>
        public TaskGroup(int id)
        {
            _id = id;
            _totalTasks = 0;
        }

        /// <summary>
        /// The ID of the task group.
        /// </summary>
        public int ID
        {
            get { return _id; }
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
                    return _totalTasks == _completedTasks;
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
                _totalTasks++;
            }
        }

        /// <summary>
        /// Notify that a task is stopping.
        /// </summary>
        public void TaskStopping()
        {
            lock (this)
            {
                _completedTasks++;
                _completeEvent.Set();
            }
        }

        /// <summary>
        /// Wait for all tasks to complete in this group.
        /// </summary>
        public void WaitForComplete()
        {
            while (!NoTasks)
            {
                _completeEvent.WaitOne();
                _completeEvent.Reset();
            }
        }
    }
}
