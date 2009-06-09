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

            PoolItem item = new PoolItem(task);
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
            int maxThreads = 0;
            int placeHolder = 0;
            int availThreads = 0;

            //Now wait until all threads from the Threadpool have returned

            while (timeOutSeconds > 0)
            {
                //figure out what the max worker thread count it

                System.Threading.ThreadPool.GetMaxThreads(out 
                             maxThreads, out placeHolder);
                System.Threading.ThreadPool.GetAvailableThreads(out availThreads,
                                                               out placeHolder);

                if (availThreads == maxThreads)
                    break;

                // Sleep
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                --timeOutSeconds;
            }
        }
    }

}
