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
