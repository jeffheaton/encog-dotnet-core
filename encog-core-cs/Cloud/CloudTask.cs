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

namespace Encog.Cloud
{
    /// <summary>
    /// Encapsulates an Encog cloud task.
    /// </summary>
    public class CloudTask
    {
        /// <summary>
        /// The cloud service.
        /// </summary>
        private EncogCloud cloud;

        /// <summary>
        /// The URL for this task.
        /// </summary>
        private String taskURL;

        /// <summary>
        /// Construct a cloud task. 
        /// </summary>
        /// <param name="cloud">The cloud this task belongs to.</param>
        public CloudTask(EncogCloud cloud)
        {
            this.cloud = cloud;
        }

        /// <summary>
        /// The cloud that this task belongs to.
        /// </summary>
        public EncogCloud Cloud
        {
            get
            {
                return this.cloud;
            }
        }

        /// <summary>
        /// Setup this task. 
        /// </summary>
        /// <param name="name">The name of this task.</param>
        public void Init(String name)
        {

            if (this.cloud.Session == null)
            {
                throw new EncogCloudError(
                        "Session must be established before a task is created.");
            }

            CloudRequest request = new CloudRequest();
            String url = this.cloud.Session;
            url += "task/create";
            IDictionary<String, String> args = new Dictionary<String, String>();
            args["name"] = name;
            args["status"] = "Starting...";
            request.PerformURLPOST(false, url, args);
            this.taskURL = this.cloud.Session + "task/"
                    + request.GetResponseProperty("id") + "/";
        }
       
        /// <summary>
        /// Set the status for this task. 
        /// </summary>
        /// <param name="status">The status for this task.</param>
        public void SetStatus(String status)
        {
            if (this.taskURL == null)
            {
                throw new EncogCloudError("Can't set status for inactive task.");
            }

            CloudRequest request = new CloudRequest();
            String url = this.taskURL + "update";

            IDictionary<String, String> args = new Dictionary<String, String>();
            args["status"] = status;
            request.PerformURLPOST(true, url, args);

        }
        
        /// <summary>
        /// Stop this task. 
        /// </summary>
        /// <param name="finalStatus">The final status for this task.</param>
        public void Stop(String finalStatus)
        {
            if (this.taskURL == null)
            {
                throw new EncogCloudError("Can't stop inactive task.");
            }

            // send final status
            CloudRequest request = new CloudRequest();
            String url = this.taskURL + "update";

            IDictionary<String, String> args = new Dictionary<String, String>();
            args["status"] = finalStatus;
            request.PerformURLPOST(false, url, args);

            // stop
            url = this.taskURL + "stop";
            request = new CloudRequest();
            request.PerformURLGET(false, url);
        }
    }
}
#endif
