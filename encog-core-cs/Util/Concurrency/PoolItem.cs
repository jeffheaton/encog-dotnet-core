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

namespace Encog.Util.Concurrency
{
    /// <summary>
    /// An item in the thread pool.
    /// </summary>
    public class PoolItem
    {
        /// <summary>
        /// The task group that this item is a part of.
        /// </summary>
        private readonly TaskGroup group;

        /// <summary>
        /// The concurrency object that started this.
        /// </summary>
        private readonly EngineConcurrency owner;

        /// <summary>
        /// The task that was executed.
        /// </summary>
        private readonly IEngineTask task;

        /// <summary>
        /// Construct a pool item.
        /// </summary>
        /// <param name="owner">The owner of this task.</param>
        /// <param name="task">The task to execute.</param>
        /// <param name="group">The group that this task belongs to.</param>
        public PoolItem(EngineConcurrency owner, IEngineTask task, TaskGroup group)
        {
            this.owner = owner;
            this.task = task;
            this.group = group;
        }

        /// <summary>
        /// The thread callback.  This actually executes the task.
        /// </summary>
        /// <param name="threadContext">The thread context, not used.</param>
        public void ThreadPoolCallback(Object threadContext)
        {
            try
            {
                task.Run();
                owner.TaskFinished(this);
            }
            finally
            {
                if (group != null)
                {
                    group.TaskStopping();
                }
            }
        }
    }
}
