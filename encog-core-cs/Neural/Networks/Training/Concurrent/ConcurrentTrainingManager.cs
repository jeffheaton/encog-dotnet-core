using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Encog.Neural.Networks.Training.Concurrent.Jobs;
using Encog.Neural.Networks.Training.Concurrent.Performers;
using Encog.Engine;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training.Strategy;
using Encog.Engine.Network.Train.Prop;
using Encog.Engine.Opencl;

namespace Encog.Neural.Networks.Training.Concurrent
{
    /// <summary>
    /// Concurrent training manager. This class allows you to queue up network
    /// training tasks to be executed either by the CPU cores or OpenCL devices. This
    /// allows the CPU/GPU to train neural networks at the same time.
    /// </summary>
    public class ConcurrentTrainingManager
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static ConcurrentTrainingManager instance;

        /// <summary>
        /// The event used to sync waiting for tasks to stop.
        /// </summary>
        private Object accessLock = new Object();

        /// <summary>
        /// Condition used to check if we are done.
        /// </summary>
        private ManualResetEvent mightBeDone = new ManualResetEvent(false);

        /// <summary>
        /// The job number.
        /// </summary>
        private int jobNumber;

        /// <summary>
        /// True, if this should be ran single threaded.
        /// </summary>
        public bool SingleThreaded { get; set; }

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static ConcurrentTrainingManager Instance
        {
            get
            {
                if (ConcurrentTrainingManager.instance == null)
                {
                    ConcurrentTrainingManager.instance = new ConcurrentTrainingManager();
                }
                return ConcurrentTrainingManager.instance;
            }
        }

        /// <summary>
        /// The performers to use.
        /// </summary>
        private IList<IConcurrentTrainingPerformer> performers = new List<IConcurrentTrainingPerformer>();

        /// <summary>
        /// The training jobs to execute.
        /// </summary>
        private IList<TrainingJob> queue = new List<TrainingJob>();

        /// <summary>
        /// The background thread.
        /// </summary>
        private Thread thread;

        /// <summary>
        /// An object used to report status.
        /// </summary>
        private IStatusReportable report = new NullStatusReportable();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private ConcurrentTrainingManager()
        {

        }
        
        /// <summary>
        /// Add a performer. 
        /// </summary>
        /// <param name="performer">The performer to add.</param>
        public void AddPerformer(IConcurrentTrainingPerformer performer)
        {
            lock (this.accessLock)
            {
                this.performers.Add(performer);
                performer.Manager = this;
            }

        }
        
        /// <summary>
        /// Add a training job. 
        /// </summary>
        /// <param name="job">The training job to add.</param>
        public void AddTrainingJob(TrainingJob job)
        {
            lock (this.accessLock)
            {
                this.queue.Add(job);
            }
        }



        /// <summary>
        /// Clear all of the performers.
        /// </summary>
        public void ClearPerformers()
        {
            lock (this.accessLock)
            {
                this.performers.Clear();
            }

        }

        /// <summary>
        /// Clear the workload.
        /// </summary>
        public void ClearQueue()
        {
            lock (this.accessLock)
            {
                this.queue.Clear();
            }
        }

        /// <summary>
        /// Detect performers. Create one performer for each OpenCL device, and
        /// another for the CPU's. If there is an OpenCL device already for the CPU,
        /// do not create another CPU performer. 
        /// </summary>
        public void DetectPerformers()
        {
            DetectPerformers(false, 0);
        }

        /// <summary>
        /// Detect performers. Create one performer for each OpenCL device, and
        /// another for the CPU's. If there is an OpenCL device already for the CPU,
        /// do not create another CPU performer. 
        /// </summary>
        /// <param name="splitCores">True, if a CPU performer should be created for each core.</param>
        /// <param name="forceCoreCount">The core count to be forced.</param>
        public void DetectPerformers(bool splitCores, int forceCoreCount)
        {
            lock (this.accessLock)
            {
                bool useCPU = true;
                ClearPerformers();

                int clCount = 1;
                int cpuCount = 1;

                this.SingleThreaded = splitCores;

                // handle OpenCL mode
                if (EncogFramework.Instance.CL != null)
                {

                    // should we let OpenCL run the CPU?
                    if (EncogFramework.Instance.CL.AreCPUsPresent())
                    {
                        useCPU = false;
                    }

                    // add a performer for each OpenCL device.
                    foreach (EncogCLDevice device in EncogFramework.Instance.CL
                            .Devices)
                    {
                        AddPerformer(new ConcurrentTrainingPerformerOpenCL(clCount++, device));
                    }
                }

                // now create CPU performers
                if (useCPU && forceCoreCount >= 0)
                {
                    int threads;

                    if (splitCores)
                    {

                        if (forceCoreCount > 0)
                            threads = forceCoreCount;
                        else
                            threads = Environment.ProcessorCount;
                    }
                    else
                    {
                        threads = 1;
                    }

                    for (int i = 0; i < threads; i++)
                    {
                        AddPerformer(new ConcurrentTrainingPerformerCPU(cpuCount++));
                    }
                }
            }
        }

        /// <summary>
        /// Wait for all tasks to finish.
        /// </summary>
        public void Join()
        {
            this.thread.Join();

        }

        /// <summary>
        /// If an error has been reported, then throw it as an exception.
        /// </summary>
        private void ReportErrors()
        {
            foreach (TrainingJob job in this.queue)
            {
                if (job.Error != null)
                {
                    throw new NeuralNetworkError(job.Error);
                }
            }
        }

        /// <summary>
        /// Perform the training.  Called internally.
        /// </summary>
        public void Run()
        {

            this.jobNumber = 0;
            this.report.Report(this.queue.Count, 0, "Starting first job");

            int count = 0;
            foreach (TrainingJob job in this.queue)
            {
                // find a performer
                IConcurrentTrainingPerformer perform = WaitForFreePerformer();
                perform.Perform(job);
                count++;
                ReportErrors();
            }

            // now wait for all performers to finish
            bool done = false;

            this.report.Report(this.queue.Count, count,
                    "No more jobs to submit, waiting for last job.");
            while (!done)
            {
                lock (this.accessLock)
                {
                    bool foundOne = false;
                    foreach (IConcurrentTrainingPerformer performer in this.performers)
                    {
                        if (!performer.Ready)
                        {
                            foundOne = true;
                        }
                    }
                    if (foundOne)
                    {
                        this.mightBeDone.WaitOne();
                    }
                    else
                    {
                        done = true;
                    }
                }
            }

            this.report.Report(this.queue.Count, count, "All training done.");
        }

        /// <summary>
        /// Report the status.
        /// </summary>
        /// <param name="str">The status to report.</param>
        private void ReportStatus(String str)
        {
            this.report.Report(this.queue.Count, jobNumber, str);

        }

        /// <summary>
        /// Setup the object to report status to. 
        /// </summary>
        public IStatusReportable Report
        {
            get
            {
                return this.report;
            }
            set
            {
                this.report = value;
            }
        }

        /// <summary>
        /// Start the manager.
        /// </summary>
        public void Start()
        {
            this.thread = new Thread(this.Run);
            this.thread.Start();
        }

        /// <summary>
        /// Wait for a free performer. 
        /// </summary>
        /// <returns>The free performer.</returns>
        public IConcurrentTrainingPerformer WaitForFreePerformer()
        {

            lock (this.accessLock)
            {
                IConcurrentTrainingPerformer result = null;

                while (result == null)
                {
                    foreach (IConcurrentTrainingPerformer performer in this.performers)
                    {
                        if (performer.Ready)
                        {
                            result = performer;
                        }
                    }

                    if (result == null)
                    {
                        this.mightBeDone.WaitOne();
                    }
                }

                return result;
            }

        }

        /// <summary>
        /// Report that a job is done.
        /// </summary>
        /// <param name="time">Time took by the job.</param>
        /// <param name="perf">The perfofmer that did the job.</param>
        public void JobDone(long time, ConcurrentTrainingPerformerCPU perf)
        {
            lock (this.accessLock)
            {
                this.jobNumber++;
                this.ReportStatus("Job finished in " + time + "ms, on " + perf.ToString());
                this.mightBeDone.Set();
                this.mightBeDone.Reset();
            }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            int index = 1;
            foreach (IConcurrentTrainingPerformer performer in this.performers)
            {
                builder.Append("Performer ");
                builder.Append(index++);
                builder.Append(": ");
                builder.Append(performer.ToString());
                builder.Append("\n");
            }
            return builder.ToString();
        }


    }
}
