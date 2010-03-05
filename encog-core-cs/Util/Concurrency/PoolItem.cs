// Encog(tm) Artificial Intelligence Framework v2.3
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

namespace Encog.Util.Concurrency
{
    /// <summary>
    /// An item in the thread pool.
    /// </summary>
    public class PoolItem
    {
        /// <summary>
        /// The task that was executed.
        /// </summary>
        private IEncogTask task;

        /// <summary>
        /// The concurrency object that started this.
        /// </summary>
        private EncogConcurrency owner;

        /// <summary>
        /// The task group that this item is a part of.
        /// </summary>
        private TaskGroup group;

        /// <summary>
        /// Construct a pool item.
        /// </summary>
        /// <param name="owner">The owner of this task.</param>
        /// <param name="task">The task to execute.</param>
        /// <param name="group">The group that this task belongs to.</param>
        public PoolItem(EncogConcurrency owner, IEncogTask task,  TaskGroup group)
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
                if (this.group != null)
                {
                    this.group.TaskStarting();
                }

                this.task.Run();
                owner.TaskFinished(this);
            }
            finally
            {
                if (this.group != null)
                {
                    this.group.TaskStopping();
                }
            }
        }
    }
}
