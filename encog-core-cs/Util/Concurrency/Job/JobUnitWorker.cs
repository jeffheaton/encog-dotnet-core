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
namespace Encog.Util.Concurrency.Job
{
    /// <summary>
    /// A worker job for Encog concurrency
    /// </summary>
    public class JobUnitWorker 
    {
        /// <summary>
        /// The context for this job unit.
        /// </summary>
        private readonly JobUnitContext _context;

        /// <summary>
        /// Construct a job unit worker to execute the specified job.
        /// </summary>
        /// <param name="context">The context of the job to execute.</param>
        public JobUnitWorker(JobUnitContext context)
        {
            _context = context;
        }

        #region IEngineTask Members

        /// <summary>
        /// Run this job.
        /// </summary>
        public void Run()
        {
            _context.Owner.PerformJobUnit(_context);
        }

        #endregion
    }
}
