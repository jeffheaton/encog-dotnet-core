using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil;

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
        /// What is the GPU workload size?
        /// </summary>
        private int gpuWorkloadSize;

        /// <summary>
        /// What is the CPU workload size?
        /// </summary>
        private int cpuWorkloadSize;

        /// <summary>
        /// GPU count.
        /// </summary>
        private int gpuWorkerCount;

        /// <summary>
        /// The total task count.
        /// </summary>
        private int totalWorkerCount;

        private List<IntRange> cpuRanges;

        private List<IntRange> gpuRanges;

        public double GPURatio { get; set; }


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
        /// Determine the workload, consider GPU count.
        /// </summary>
        /// <param name="cpuWorkerCount">Threads to use, or zero to allow Encog to pick.</param>
        /// <param name="gpuWorkerCount">The number of GPU workers.</param>
        /// <param name="workloadSize">Total workload size.</param>
        public DetermineWorkload(int cpuWorkerCount, int gpuWorkerCount, int workloadSize)
        {
            this.cpuWorkerCount = cpuWorkerCount;
            this.gpuWorkerCount = gpuWorkerCount;
            this.totalWorkerCount = gpuWorkerCount + cpuWorkerCount;
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
                this.totalWorkerCount = this.gpuWorkerCount + this.cpuWorkerCount;
            }
            else
            {
                this.totalWorkerCount = gpuWorkerCount + cpuWorkerCount;
                if (this.totalWorkerCount > workloadSize)
                    this.gpuWorkerCount = 0;
                this.totalWorkerCount = gpuWorkerCount + cpuWorkerCount;
                this.totalWorkerCount = Math.Min(this.totalWorkerCount, workloadSize);
            }
        }


        /// <summary>
        /// Calculate the high and low ranges for each worker.
        /// </summary>
        public void CalculateWorkers()
        {
            this.cpuRanges.Clear();
            this.gpuRanges.Clear();

            int baseSizePerThread = this.totalWorkloadSize / this.totalWorkerCount;
            int gpuSizePerThread = (int)((double)baseSizePerThread * this.GPURatio);
            int cpuWorkloadSize = this.totalWorkloadSize - (gpuSizePerThread * this.gpuWorkerCount);
            int cpuSizePerThread = cpuWorkloadSize / this.cpuWorkerCount;

            int index = 0;

            // create the GPU workers
            for (int i = 0; i < this.gpuWorkerCount; i++)
            {
                int low = index;
                int high = low + gpuSizePerThread;
                this.gpuRanges.Add(new IntRange(high, low));
                index += gpuSizePerThread;
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
                    high = low + cpuSizePerThread;
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
        /// The GPU thread count.
        /// </summary>
        public int GPUWorkerCount
        {
            get
            {
                return this.gpuWorkerCount;
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
        public List<IntRange> CPURanges
        {
            get
            {
                return this.cpuRanges;
            }
        }

        /// <summary>
        /// Workload ranges for GPU workers.
        /// </summary>
        public List<IntRange> GPURanges
        {
            get
            {
                return this.gpuRanges;
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
        /// What is the GPU workload size?
        /// </summary>
        private int GPUWorkloadSize
        {
            get
            {
                return this.gpuWorkloadSize;
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
