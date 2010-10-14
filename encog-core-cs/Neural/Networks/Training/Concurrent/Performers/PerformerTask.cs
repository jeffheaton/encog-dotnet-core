using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Concurrency.Job;

namespace Encog.Neural.Networks.Training.Concurrent.Performers
{
    /// <summary>
    /// Perform one training task.
    /// </summary>
    public class PerformerTask : IEngineTask
    {
        /// <summary>
        /// The owner.
        /// </summary>
        private IConcurrentTrainingPerformer owner;

        /// <summary>
        /// Construct the task runner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public PerformerTask(IConcurrentTrainingPerformer owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Run the test.
        /// </summary>
        public void Run()
        {
            this.owner.Run();
        }
    }
}
