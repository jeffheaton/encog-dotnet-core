//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
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
using System.Threading;

#if logging

#endif

namespace Encog.Util.Concurrency
{
    /// <summary>
    /// This class abstracts thread pools, and potentially grids and other types of
    /// concurrency. It is used by other classes inside of Encog to allow tasks to be
    /// executed efficiently on multicore machines.
    /// </summary>
    public class EngineConcurrency: IMultiThreadable
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static EngineConcurrency _instance = new EngineConcurrency();

        /// <summary>
        /// The number of active tasks.
        /// </summary>
        private int _activeTasks;

        private int _currentTaskGroup;

        /// <summary>
        /// The instance to the singleton.
        /// </summary>
        public static EngineConcurrency Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Construct a concurrency object.
        /// </summary>
        public EngineConcurrency()
        {
            SetMaxThreadsToCoreCount();
            _currentTaskGroup = 0;
        }

        /// <summary>
        /// Process the specified task.  It will be processed either now,
        /// or queued to process on the thread pool.  No group is assigned.
        /// </summary>
        /// <param name="task">The task to process.</param>
        public void ProcessTask(IEngineTask task)
        {
            ProcessTask(task, null);
        }

        /// <summary>
        /// Process the specified task.  It will be processed either now,
        /// or queued to process on the thread pool.
        /// </summary>
        /// <param name="task">The task to process.</param>
        /// <param name="group">The group this task belongs to.</param>
        public void ProcessTask(IEngineTask task, TaskGroup group)
        {
            lock (this)
            {
                _activeTasks++;
            }
            if (group != null)
                group.TaskStarting();
            var item = new PoolItem(this, task, group);
            ThreadPool.QueueUserWorkItem(item.ThreadPoolCallback);
        }

        /// <summary>
        /// How many threads should be used?
        /// </summary>
        public int MaxThreads
        {
            get
            {
                int t1, t2;
                ThreadPool.GetMaxThreads(out t1, out t2);
                return t1;
            }
            set
            {
                int threads = value;

                if (threads == 0)
                {
                    threads = Environment.ProcessorCount;
                    if (threads > 1)
                        threads++;
                }

                ThreadPool.SetMaxThreads(threads, threads);
            }
        }

        /// <summary>
        /// Set the max threads to the number of processors.
        /// </summary>
        public void SetMaxThreadsToCoreCount()
        {
            MaxThreads = Environment.ProcessorCount;
        }

        /// <summary>
        /// Create a new task group.
        /// </summary>
        /// <returns>The new task group.</returns>
        public TaskGroup CreateTaskGroup()
        {
            TaskGroup result;
            lock (this)
            {
                _currentTaskGroup++;
                result = new TaskGroup(_currentTaskGroup);
            }
            return result;
        }

        internal void TaskFinished(PoolItem poolItem)
        {
            lock (this)
            {
                _activeTasks--;
            }
        }

        public int ThreadCount { get; set; }
    }
}
