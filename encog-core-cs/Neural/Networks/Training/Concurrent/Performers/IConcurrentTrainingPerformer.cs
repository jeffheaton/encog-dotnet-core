using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Training.Concurrent.Jobs;

namespace Encog.Neural.Networks.Training.Concurrent.Performers
{
    /// <summary>
    /// Performers actually perform the training. Currently there are performers for
    /// OpenCL and CPU.
    /// </summary>
    public interface IConcurrentTrainingPerformer
    {
        /// <summary>
        /// True, if this performer is ready to train a job.
        /// </summary>
        bool Ready { get; }
        
        /// <summary>
        /// Perform the specified job. 
        /// </summary>
        /// <param name="job">The job to perform.</param>
        void Perform(TrainingJob job);


        /// <summary>
        /// Set the manager.
        /// </summary>
        ConcurrentTrainingManager Manager { get; set; }

        /// <summary>
        /// Perform the current job.
        /// </summary>
        void Run();
    }
}
