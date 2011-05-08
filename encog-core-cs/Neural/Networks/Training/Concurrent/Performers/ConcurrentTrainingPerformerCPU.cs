using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Training.Concurrent.Jobs;
using Encog.Engine.Concurrency.Job;
using System.Diagnostics;
using Encog.Engine.Network.Train.Prop;
using Encog.Util.Time;

namespace Encog.Neural.Networks.Training.Concurrent.Performers
{
    /// <summary>
    /// This performer allows jobs to be performed by the CPU.
    /// </summary>
    public class ConcurrentTrainingPerformerCPU : IConcurrentTrainingPerformer
    {
        /// <summary>
        /// True, if this performer is ready for more work.
        /// </summary>
        private bool ready = true;

        /// <summary>
        /// The current job.
        /// </summary>
        private TrainingJob currentJob;

        /// <summary>
        /// The manager.
        /// </summary>
        public ConcurrentTrainingManager Manager { get; set; }

        /// <summary>
        /// The CPU number.
        /// </summary>
        private int number;

        /// <summary>
        /// Construct a CPU performer.
        /// </summary>
        /// <param name="number">The CPU number.</param>
        public ConcurrentTrainingPerformerCPU(int number)
        {
            this.number = number;
        }

        /// <inheritdoc/>
        public void Perform(TrainingJob job)
        {

            lock (this)
            {
                if (!this.ready)
                {
                    throw new NeuralNetworkError(
                            "Performer is already performing a job.");
                }

                this.ready = false;
            }

            this.currentJob = job;

            PerformerTask task = new PerformerTask(this);
            EngineConcurrency.Instance.ProcessTask(task);
        }

        /// <inheritdoc/>
        public bool Ready
        {
            get
            {
                lock (this)
                {
                    return this.ready;
                }
            }
        }

        /// <inheritdoc/>
        public void Run()
        {
            Stopwatch watch = new Stopwatch();
            try
            {
                watch.Start();

                this.currentJob.CreateTrainer(Manager.SingleThreaded);
                ITrain train = this.currentJob.Train;
                int interation = 1;

                while (this.currentJob.ShouldContinue())
                {
                    train.Iteration(this.currentJob.IterationsPer);
                    interation++;
                }
                watch.Stop();
            }
            catch (Exception t)
            {
                this.currentJob.Error = t;
            }
            finally
            {
                lock (this)
                {
                    this.ready = true;
                }
                this.Manager.JobDone(watch.ElapsedMilliseconds, this);
            }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            return "[CPU-Performer: " + this.number + "]";
        }

        /// <summary>
        /// The device number.
        /// </summary>
        public int Number
        {
            get
            {
                return number;
            }
        }


    }
}
