using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil;
using Encog.Neural;

namespace Encog.Util.Concurrency
{
    /// <summary>
    /// Used by several Encog training methods to break up a workload. Can also be
    /// used to determine the number of threads to use. If zero threads are
    /// specified, Encog will query the processor count and determine the best number
    /// of threads to use.
    /// </summary>
    public class DetermineWorkload
    {
        /// <summary>
        /// What is the minimum number of workload entries for a thread to be worthwhile.
        /// </summary>
        public const int MIN_WORTHWHILE = 100;

        /// <summary>
        /// How many threads to use.
        /// </summary>
        private int cpuWorkerCount;

        /// <summary>
        /// What is the total workload size?
        /// </summary>
        private int totalWorkloadSize;

        /// <summary>
        /// What is the CL workload size?
        /// </summary>
        private int clWorkloadSize;

        /// <summary>
        /// What is the CPU workload size?
        /// </summary>
        private int cpuWorkloadSize;

        /// <summary>
        /// CL worker count.
        /// </summary>
        private int clWorkerCount;

        /// <summary>
        /// The total task count.
        /// </summary>
        private int totalWorkerCount;

        private IList<IntRange> cpuRanges = new List<IntRange>();

        private IList<IntRange> clRanges = new List<IntRange>();

        public double CLRatio { get; set; }


        /// <summary>
        /// Determine the workload.
        /// </summary>
        /// <param name="threads">Threads to use, or zero to allow Encog to pick.</param>
        /// <param name="workloadSize">Total workload size.</param>
        public DetermineWorkload(int threads, int workloadSize) :
            this(threads,0,workloadSize)
        {
        }

        /// <summary>
        /// Determine the workload, consider CL count.
        /// </summary>
        /// <param name="cpuWorkerCount">Threads to use, or zero to allow Encog to pick.</param>
        /// <param name="clWorkerCount">The number of CL workers.</param>
        /// <param name="workloadSize">Total workload size.</param>
        public DetermineWorkload(int cpuWorkerCount, int clWorkerCount, int workloadSize)
        {
            this.CLRatio = 1;
            this.cpuWorkerCount = cpuWorkerCount;
            this.clWorkerCount = clWorkerCount;
            this.totalWorkerCount = clWorkerCount + cpuWorkerCount;
            this.totalWorkloadSize = workloadSize;

            if (cpuWorkerCount == 0)
            {
                int num = System.Environment.ProcessorCount;

                // if there is more than one processor, use processor count +1
                if (num != 1)
                {
                    num++;
                }
                // if there is a single processor, just use one thread

                // Now see how big the training sets are going to be.
                // We want at least 100 training elements in each.
                // This method will likely be further "tuned" in future versions.

                long recordCount = this.totalWorkloadSize;
                long workPerThread = recordCount / num;

                if (workPerThread < DetermineWorkload.MIN_WORTHWHILE )
                {
                    num = Math.Max(1, (int)(recordCount / DetermineWorkload.MIN_WORTHWHILE));
                }

                this.cpuWorkerCount = num;
                this.totalWorkerCount = this.clWorkerCount + this.cpuWorkerCount;
            }
            else
            {
                if (cpuWorkerCount == -1)
                    cpuWorkerCount = 0;

                this.totalWorkerCount = clWorkerCount + cpuWorkerCount;
                if (this.totalWorkerCount > workloadSize)
                    this.clWorkerCount = 0;
                this.totalWorkerCount = clWorkerCount + cpuWorkerCount;
                this.totalWorkerCount = Math.Min(this.totalWorkerCount, workloadSize);
            }
        }


        /// <summary>
        /// Calculate the high and low ranges for each worker.
        /// </summary>
        public void CalculateWorkers()
        {
            this.cpuRanges.Clear();
            this.clRanges.Clear();

            if (this.totalWorkerCount == 0)
                throw new NeuralNetworkError("Can't train with zero workers.");

            int baseSizePerThread = this.totalWorkloadSize / this.totalWorkerCount;
            int clSizePerThread = (int)((double)baseSizePerThread * this.CLRatio);
            int cpuWorkloadSize = Math.Max( this.totalWorkloadSize - (clSizePerThread * this.clWorkerCount), 0);
            int cpuSizePerThread = Math.Max(cpuWorkloadSize / this.cpuWorkerCount, 0);

            int index = 0;

            // create the CL workers
            for (int i = 0; i < this.clWorkerCount; i++)
            {
                int low = index;
                int high = (low + clSizePerThread)-1;
                this.clRanges.Add(new IntRange(high, low));
                index += clSizePerThread;
            }

            // create the CPU workers
            for (int i = 0; i < this.cpuWorkerCount; i++)
            {
                int low = index;
                int high;

                // if this is the last record, then high to be the last item
                // in the training set.
                if (i == (this.cpuWorkerCount - 1))
                {
                    high = this.totalWorkloadSize - 1;
                }
                else
                {
                    high = (low + cpuSizePerThread)-1;
                }

                this.cpuRanges.Add(new IntRange(high, low));
                index += cpuSizePerThread;
            }

        }

        /// <summary>
        /// The thread count.
        /// </summary>
        public int TotalWorkerCount
        {
            get
            {
                return this.totalWorkerCount;
            }
        }
        
        /// <summary>
        /// The CL thread count.
        /// </summary>
        public int CLWorkerCount
        {
            get
            {
                return this.clWorkerCount;
            }
        }

        /// <summary>
        /// The thread count.
        /// </summary>
        public int CPUWorkerCount
        {
            get
            {
                return this.cpuWorkerCount;
            }
        }

        /// <summary>
        /// Workload ranges for CPU workers.
        /// </summary>
        public IList<IntRange> CPURanges
        {
            get
            {
                return this.cpuRanges;
            }
        }

        /// <summary>
        /// Workload ranges for CL workers.
        /// </summary>
        public IList<IntRange> CLRanges
        {
            get
            {
                return this.clRanges;
            }
        }

        /// <summary>
        /// What is the total workload size?
        /// </summary>
        private int TotalWorkloadSize
        {
            get
            {
                return this.totalWorkloadSize;
            }
        }

        /// <summary>
        /// What is the CL workload size?
        /// </summary>
        private int CLWorkloadSize
        {
            get
            {
                return this.clWorkloadSize;
            }
        }

        /// <summary>
        /// What is the CPU workload size?
        /// </summary>
        private int CPUWorkloadSize
        {
            get
            {
                return this.cpuWorkloadSize;
            }
        }

    }
}
