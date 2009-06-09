using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Concurrency
{
    class PoolItem
    {
        private IEncogTask task;

        public PoolItem(IEncogTask task)
        {
            this.task = task;
        }

        public void ThreadPoolCallback(Object threadContext)
        {
            this.task.Run();
        }
    }
}
