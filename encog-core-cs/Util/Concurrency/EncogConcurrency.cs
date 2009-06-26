using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Threading;

namespace Encog.Util.Concurrency
{
    /// <summary>
    /// This class abstracts thread pools, and potentially grids and other types of
    /// concurrency. It is used by other classes inside of Encog to allow tasks to be
    /// executed efficiently on multicore machines.
    /// </summary>
    public class EncogConcurrency
    {

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static EncogConcurrency instance;

        /// <summary>
        /// The number of active tasks.
        /// </summary>
        private int activeTasks;

        /// <summary>
        /// The instance to the singleton.
        /// </summary>
        public static EncogConcurrency Instance
        {
            get
            {
                if (EncogConcurrency.instance == null)
                {
                    EncogConcurrency.instance = new EncogConcurrency();
                }
                return EncogConcurrency.instance;
            }
        }

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(EncogConcurrency));

        /// <summary>
        /// Construct a concurrency object.
        /// </summary>
        public EncogConcurrency()
        {
            SetMaxThreadsToCoreCount();
        }

        /// <summary>
        /// Process the specified task.  It will be processed either now,
        /// or queued to process on the thread pool.
        /// </summary>
        /// <param name="task">The task to process.</param>
        public void ProcessTask(IEncogTask task)
        {
            lock (this)
            {
                this.activeTasks++;
            }

            PoolItem item = new PoolItem(this, task);
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
                ThreadPool.SetMaxThreads(value, value);
            }
        }

        /// <summary>
        /// Set the max threads to the number of processors.
        /// </summary>
        public void SetMaxThreadsToCoreCount()
        {
            this.MaxThreads = System.Environment.ProcessorCount;
        }

        /// <summary>
        /// Wait for all threads in the pool to complete.
        /// </summary>
        /// <param name="timeOutSeconds">How long to wait for all threads to complete.</param>
        public void Shutdown(long timeOutSeconds)
        {
            long start = DateTime.Now.Ticks;
            long current;
            long elapsed;

            do
            {
                lock (this)
                {
                    if (this.activeTasks < 1)
                        return;
                }
                current = DateTime.Now.Ticks;
                elapsed = current - start;
                elapsed /= 10000; // to miliseconds
                elapsed /= 1000; // to seconds
                Thread.Sleep(100);
            } while (elapsed < timeOutSeconds);
        }

        internal void TaskFinished(PoolItem poolItem)
        {
            lock (this)
            {
                this.activeTasks--;
            }
        }
    }

}
