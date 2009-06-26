using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Concurrency
{
    class PoolItem
    {
        private IEncogTask task;
        private EncogConcurrency owner;

        public PoolItem(EncogConcurrency owner, IEncogTask task)
        {
            this.owner = owner;
            this.task = task;
        }

        public void ThreadPoolCallback(Object threadContext)
        {
            this.task.Run();
            owner.TaskFinished(this);
        }
    }
}
