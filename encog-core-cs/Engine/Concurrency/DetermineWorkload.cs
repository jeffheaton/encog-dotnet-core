/*
 * Encog(tm) Core v2.5 - Java Version
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 
 * Copyright 2008-2010 Heaton Research, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *   
 * For more information on Heaton Research copyrights, licenses 
 * and trademarks visit:
 * http://www.heatonresearch.com/copyright
 */

namespace Encog.Engine.Concurrency
{

    using Encog.Engine.Util;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Used by several Encog training methods to break up a workload. Can also be
    /// used to determine the number of threads to use. If zero threads are
    /// specified, Encog will query the processor count and determine the best number
    /// of threads to use.
    /// </summary>
    ///
    public class DetermineWorkload
    {

        /// <summary>
        /// What is the minimum number of workload entries for a thread to be
        /// worthwhile.
        /// </summary>
        ///
        public const int MIN_WORTHWHILE = 100;

        /// <summary>
        /// How many threads to use.
        /// </summary>
        ///
        private int threadCount;

        /// <summary>
        /// What is the total workload size?
        /// </summary>
        ///
        private int workloadSize;

        /// <summary>
        /// Determine the workload.
        /// </summary>
        ///
        /// <param name="threads">Threads to use, or zero to allow Encog to pick.</param>
        /// <param name="workloadSize">Total workload size.</param>
        public DetermineWorkload(int threads, int workloadSize)
        {

            this.workloadSize = workloadSize;
            if (threads == 0)
            {
#if !SILVERLIGHT
                int num = (int)(Math.Log((double)((int)System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity + 1), 2));
#else
                int num = 4;
#endif

                // if there is more than one processor, use processor count +1
                if (num != 1)
                {
                    num++;
                }
                // if there is a single processor, just use one thread

                // Now see how big the training sets are going to be.
                // We want at least 100 training elements in each.
                // This method will likely be further "tuned" in future versions.

                long recordCount = this.workloadSize;
                long workPerThread = recordCount / num;

                if (workPerThread < 100)
                {
                    num = Math.Max(1, (int)(recordCount / 100));
                }

                this.threadCount = num;
            }
            else
            {
                this.threadCount = Math.Min(threads, workloadSize);
            }
        }

        /// <summary>
        /// Calculate the high and low ranges for each worker.
        /// </summary>
        /// <returns>A list of IntRange objects.</returns>
        public IList<IntRange> CalculateWorkers()
        {
            IList<IntRange> result = new List<IntRange>();
            int sizePerThread = this.workloadSize / this.threadCount;

            // create the workers
            for (int i = 0; i < this.threadCount; i++)
            {
                int low = i * sizePerThread;
                int high;

                // if this is the last record, then high to be the last item
                // in the training set.
                if (i == (this.threadCount - 1))
                {
                    high = this.workloadSize - 1;
                }
                else
                {
                    high = ((i + 1) * sizePerThread) - 1;
                }

                result.Add(new IntRange(high, low));
            }

            return result;
        }


        /// <summary>
        /// The thread count.
        /// </summary>
        public int ThreadCount
        {
            get
            {
                return this.threadCount;
            }
        }

    }
}
