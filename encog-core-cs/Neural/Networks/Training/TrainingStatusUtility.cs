// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Cloud;
using Encog.Util;
using Encog.Engine.Util;

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
#endif
