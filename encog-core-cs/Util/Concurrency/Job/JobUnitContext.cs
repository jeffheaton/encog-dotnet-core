using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Concurrency.Job
{
    public class JobUnitContext
    {

        private Object jobUnit;
        private ConcurrentJob owner;
        private int taskNumber;

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
