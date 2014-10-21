//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Encog.MathUtil;

namespace Encog.Util.Concurrency
{
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
        public const int MinWorthwhile = 100;

        /// <summary>
        /// How many threads to use.
        /// </summary>
        ///
        private readonly int _threadCount;

        /// <summary>
        /// What is the total workload size?
        /// </summary>
        ///
        private readonly int _workloadSize;

        /// <summary>
        /// Determine the workload.
        /// </summary>
        ///
        /// <param name="threads">Threads to use, or zero to allow Encog to pick.</param>
        /// <param name="workloadSize">Total workload size.</param>
        public DetermineWorkload(int threads, int workloadSize)
        {
            _workloadSize = workloadSize;
            if (threads == 0)
            {
                var num = (int) (Math.Log(((int) Process.GetCurrentProcess().ProcessorAffinity + 1), 2));

                // if there is more than one processor, use processor count +1
                if (num != 1)
                {
                    num++;
                }
                // if there is a single processor, just use one thread

                // Now see how big the training sets are going to be.
                // We want at least 100 training elements in each.
                // This method will likely be further "tuned" in future versions.

                long recordCount = _workloadSize;
                long workPerThread = recordCount/num;

                if (workPerThread < 100)
                {
                    num = Math.Max(1, (int) (recordCount/100));
                }

                _threadCount = num;
            }
            else
            {
                _threadCount = Math.Min(threads, workloadSize);
            }
        }


        /// <summary>
        /// The thread count.
        /// </summary>
        public int ThreadCount
        {
            get { return _threadCount; }
        }

        /// <summary>
        /// Calculate the high and low ranges for each worker.
        /// </summary>
        /// <returns>A list of IntRange objects.</returns>
        public IList<IntRange> CalculateWorkers()
        {
            IList<IntRange> result = new List<IntRange>();
            int sizePerThread = _workloadSize/_threadCount;

            // create the workers
            for (int i = 0; i < _threadCount; i++)
            {
                int low = i*sizePerThread;
                int high;

                // if this is the last record, then high to be the last item
                // in the training set.
                if (i == (_threadCount - 1))
                {
                    high = _workloadSize - 1;
                }
                else
                {
                    high = ((i + 1)*sizePerThread) - 1;
                }

                result.Add(new IntRange(high, low));
            }

            return result;
        }
    }
}
