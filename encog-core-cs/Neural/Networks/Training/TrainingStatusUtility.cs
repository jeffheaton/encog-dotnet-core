using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Cloud;
using Encog.Util;

namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// Report the status of Encog training to the cloud.
    /// </summary>
    public class TrainingStatusUtility
    {
        /// <summary>
        /// The last time an update was performed.
        /// </summary>
        private long lastUpdate;

        /// <summary>
        /// The iteration number.
        /// </summary>
        private int iteration;

        /// <summary>
        /// The training object.
        /// </summary>
        private ITrain train;

        /// <summary>
        /// The cloud.
        /// </summary>
        private EncogCloud cloud;

        /// <summary>
        /// The task that we are reporting to.
        /// </summary>
        private CloudTask task;


        /// <summary>
        /// Create a training status utility.
        /// </summary>
        /// <param name="cloud">The cloud to report to.</param>
        /// <param name="train">The training object being used.</param>
        public TrainingStatusUtility(EncogCloud cloud, ITrain train)
        {
            this.cloud = cloud;
            this.train = train;
            this.lastUpdate = 0;
            this.iteration = 0;
        }

        /// <summary>
        /// Perform an update.
        /// </summary>
        public void Update()
        {
            long now = Environment.TickCount;
            long elapsed = (long)TimeSpan.FromTicks(now - lastUpdate).TotalSeconds;

            if (task == null)
            {
                this.task = this.cloud.BeginTask(train.GetType().Name);
            }

            this.iteration++;

            if (elapsed > 10)
            {
                this.lastUpdate = now;
                StringBuilder status = new StringBuilder();
                status.Append("Iteration #");
                status.Append(Format.FormatInteger(this.iteration));
                status.Append(" - Error: ");
                status.Append(Format.FormatPercent(train.Error));
                this.task.SetStatus(status.ToString());
            }
        }

        /// <summary>
        /// Report that we are finished.
        /// </summary>
        public void Finish()
        {
            StringBuilder status = new StringBuilder();
            status.Append("Done at iteration #");
            status.Append(Format.FormatInteger(this.iteration));
            status.Append(" - Error: ");
            status.Append(Format.FormatPercent(train.Error));

            this.task.Stop(status.ToString());
            this.task = null;
        }

    }
}
