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
    public class AsynchronousCloudRequest
    {
        /**
 * The URL that the request was sent to.
 */
        private String url;

        /**
         * The parameters for the request.
         */
        private IDictionary<String, String> param;

        /**
         * Construct the cloud request.  Used for a simple GET request.
         * @param url The URL this request is to go to.
         */
        public AsynchronousCloudRequest(String url)
        {
            this.url = url;
            this.param = new Dictionary<String, String>();
        }

        /**
         * Construct the cloud request.  Used for a POST request.
         * @param url The URL this request goes to.
         * @param params The POST params.
         */
        public AsynchronousCloudRequest(String url,
                 IDictionary<String, String> param)
        {
            this.url = url;
            this.param = param;
        }

        /**
         * @return The POST params.
         */
        public IDictionary<String, String> getParams()
        {
            return this.param;
        }

        /**
         * @return The URL this request is going to.
         */
        public String getUrl()
        {
            return this.url;
        }

        /**
         * Ran by the thread to perform the request.
         */
        public void run()
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
