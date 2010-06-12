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
    public class EncogCloud
    {
        /// <summary>
        /// The default cloud server.
        /// </summary>
        public const String DEFAULT_SERVER = "http://cloud.encog.com/";

        /// <summary>
        /// The session.
        /// </summary>
        private String session;

        /// <summary>
        /// The server.
        /// </summary>
 
        private String server;

        /// <summary>
        /// Construct an Encog cloud connection. The connection will not be
        /// established until the connect method is called. 
        /// </summary>
        /// <param name="server">The server to connect to.</param>
        public EncogCloud(String server)
        {
            this.server = server;
            if (!this.server.EndsWith("/"))
            {
                this.server += '/';
            }
        }


        /// <summary>
        /// Begin a task with the specified name. 
        /// </summary>
        /// <param name="name">The name of the task to begin.</param>
        /// <returns>The new task.</returns>
        public CloudTask BeginTask(String name)
        {
            CloudTask result = new CloudTask(this);
            result.Init(name);

            return result;
        }

        /// <summary>
        /// Connect to the Encog cloud.
        /// </summary>
        /// <param name="uid">The user id.</param>
        /// <param name="pwd">The password.</param>
        public void Connect(String uid, String pwd)
        {
            CloudRequest request = new CloudRequest();
            IDictionary<String, String> args = new Dictionary<String, String>();
            args["uid"] = uid;
            args["pwd"] = pwd;
            request.PerformURLPOST(false, ConstructService("login"), args);
            if (!"success".Equals(request.Status))
            {
                throw new EncogCloudError(request.Message);
            }
            this.session = request.Session;
        }

        /// <summary>
        /// Construct a string that connects to the specified service.
        /// </summary>
        /// <param name="service">The service to connect to.</param>
        /// <returns>The complete URL.</returns>
        public String ConstructService(String service)
        {
            return this.server + service;
        }

        /// <summary>
        /// The session we are connected to.
        /// </summary>
        public String Session
        {
            get
            {
                return this.session;
            }
        }

        /// <summary>
        /// True if we are connected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.session != null;
            }
        }

        /// <summary>
        /// Logout of the specified session.
        /// </summary>
        public void Logout()
        {
            CloudRequest request = new CloudRequest();
            request.PerformURLGET(false, this.session + "logout");
            this.session = null;
        }

        /// <summary>
        /// Validate the session.
        /// </summary>
        /// <param name="failOnError">True if an exception should be thrown on error.</param>
        public void ValidateSession(bool failOnError)
        {
            int max;

            if (failOnError)
            {
                max = 1;
            }
            else
            {
                max = 5;
            }

            for (int i = 0; i < max; i++)
            {
                CloudRequest request = new CloudRequest();
                request.PerformURLGET(false, this.session);
                if ("success".Equals(request.Status))
                {
                    return;
                }
            }

            if (failOnError)
            {
                throw new EncogCloudError("Connection lost");
            }
        }
    }
}
#endif