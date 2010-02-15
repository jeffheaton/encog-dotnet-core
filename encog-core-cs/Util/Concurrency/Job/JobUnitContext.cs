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

namespace Encog.Util.Concurrency.Job
{
    /// <summary>
    /// Holds basic configuration information for an Encog job.
    /// </summary>
    public class JobUnitContext
    {
        /// <summary>
        /// The job unit that this job will execute.
        /// </summary>
        private Object jobUnit;

        /// <summary>
        /// The owner of this job.
        /// </summary>
        private ConcurrentJob owner;

        /// <summary>
        /// The task number.
        /// </summary>
        private int taskNumber;

        /// <summary>
        /// The JobUnit that this context will execute.
        /// </summary>
        public Object JobUnit
        {
            get
            {
                return jobUnit;
            }
            set
            {
                this.jobUnit = value;
            }
        }

        /// <summary>
        /// The owner of this job.
        /// </summary>
        public ConcurrentJob Owner
        {
            get
            {
                return owner;
            }
            set
            {
                this.owner = value;
            }
        }


        /// <summary>
        /// The task number.
        /// </summary>
        public int TaskNumber
        {
            get
            {
                return taskNumber;
            }
            set
            {
                this.taskNumber = value;
            }
        }
    }
}
