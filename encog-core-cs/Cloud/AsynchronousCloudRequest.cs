using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Bot;
using System.Net;
using System.IO;
using Encog.Util.HTTP;

namespace Encog.Cloud
{
    /// <summary>
    /// An asynchronous cloud request is set to the cloud, but we do not require a
    /// reply. This allows status updates to be executed in a separate therad without
    /// the need to wait.
    /// </summary>
    public class AsynchronousCloudRequest
    {
        /// <summary>
        /// The URL that the request was sent to.
        /// </summary>
        private String url;

        /// <summary>
        /// The parameters for the request.
        /// </summary>
        private IDictionary<String, String> param;

        /// <summary>
        /// Construct the cloud request.  Used for a simple GET request.
        /// </summary>
        /// <param name="url">The URL this request is to go to.</param>
        public AsynchronousCloudRequest(String url)
        {
            this.url = url;
            this.param = new Dictionary<String, String>();
        }

        /// <summary>
        /// Construct the cloud request.  Used for a POST request.
        /// </summary>
        /// <param name="url">The URL this request goes to.</param>
        /// <param name="param">The POST params.</param>
        public AsynchronousCloudRequest(String url,
                 IDictionary<String, String> param)
        {
            this.url = url;
            this.param = param;
        }

        /// <summary>
        /// The POST params.
        /// </summary>
        public IDictionary<String, String> Params
        {
            get
            {
                return this.param;
            }
        }

        /// <summary>
        /// The URL this request is going to.
        /// </summary>
        public String Url
        {
            get
            {
                return this.url;
            }
        }

        /// <summary>
        /// Ran by the thread to perform the request.
        /// </summary>
        public void Run()
        {
            if (this.param.Count > 0)
            {
                BotUtil.POSTPage(new Uri(this.url), this.param);
            }
            else
            {
                BotUtil.LoadPage(new Uri(url));
            }
        }
    }
}
