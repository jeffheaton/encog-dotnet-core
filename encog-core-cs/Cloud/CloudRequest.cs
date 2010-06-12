#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Parse.Tags.Read;
using System.IO;
using System.Threading;
using Encog.Bot;

namespace Encog.Cloud
{
    /// <summary>
    /// An Encog cloud request.  Sends a request to the Encog cloud and handles the response.
    /// </summary>
    public class CloudRequest
    {
        /// <summary>
        /// The header properties.
        /// </summary>
        private IDictionary<String, String> headerProperties = new Dictionary<String, String>();

        /// <summary>
        /// The session properties.
        /// </summary>
        private IDictionary<String, String> sessionProperties = new Dictionary<String, String>();

        /// <summary>
        /// The response properties.
        /// </summary>
        private IDictionary<String, String> responseProperties = new Dictionary<String, String>();

        /// <summary>
        /// The message returned from the cloud.
        /// </summary>
        public String Message
        {
            get
            {
                if (this.headerProperties.ContainsKey("message"))
                    return this.headerProperties["message"];
                else
                    return null;
            }
        }


        /// <summary>
        /// Get a response property. 
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The property.</returns>
        public String GetResponseProperty(String key)
        {
            if (this.responseProperties.ContainsKey(key))
                return this.responseProperties[key];
            else
                return null;
        }

        /// <summary>
        /// The service.
        /// </summary>
        public String Service
        {
            get
            {
                if (this.headerProperties.ContainsKey("service"))
                    return this.headerProperties["service"];
                else
                    return null;
            }
        }

        /// <summary>
        /// The session.
        /// </summary>
        public String Session
        {
            get
            {
                if (this.sessionProperties.ContainsKey("url"))
                    return this.sessionProperties["url"];
                else
                    return null;
            }
        }

        /// <summary>
        /// The status.
        /// </summary>
        public String Status
        {
            get
            {
                if (this.headerProperties.ContainsKey("status"))
                    return this.headerProperties["status"];
                else
                    return null;
            }
        }

        /// <summary>
        /// Handle the cloud response. 
        /// </summary>
        /// <param name="contents">The contents.</param>
        private void HandleResponse(String contents)
        {
            byte[] bin = System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(contents);
            MemoryStream istream = new MemoryStream(bin);
            ReadXML xml = new ReadXML(istream);
            int ch;

            while ((ch = xml.Read()) != -1)
            {
                if (ch == 0)
                {
                    if (xml.LastTag.Name.Equals("EncogCloud"))
                    {
                        ProcessCloud(xml);
                    }
                }
            }

            if ((Status == null) || Status.Equals("failed"))
            {
                throw new EncogCloudError(Message);
            }
        }

        /// <summary>
        /// Perform a GET request. 
        /// </summary>
        /// <param name="async">True if this request should be asynchronous.</param>
        /// <param name="url">The URL.</param>
        public void PerformURLGET(bool async, String url)
        {
            try
            {
                if (async)
                {
                    AsynchronousCloudRequest request = new AsynchronousCloudRequest(url);
                    Thread t = new Thread(new ThreadStart(request.Run));
                    t.Start();
                }
                else
                {
                    String contents = BotUtil.LoadPage(new Uri(url));
                    HandleResponse(contents);
                }
            }
            catch (IOException e)
            {
                throw new EncogCloudError(e);
            }
        }

        /// <summary>
        /// Perform a POST to the cloud. 
        /// </summary>
        /// <param name="async">True if this request should be asynchronous.</param>
        /// <param name="service">The service.</param>
        /// <param name="args">The POST arguments.</param>
        public void PerformURLPOST(bool async, String service,
                IDictionary<String, String> args)
        {
            try
            {
                if (async)
                {
                    AsynchronousCloudRequest request = new AsynchronousCloudRequest(
                            service, args);
                    Thread t = new Thread(new ThreadStart(request.Run));
                    t.Start();
                }
                else
                {
                    String contents = BotUtil.POSTPage(new Uri(service), args);
                    HandleResponse(contents);
                }
            }
            catch (IOException e)
            {
                throw new EncogCloudError(e);
            }
        }


        /// <summary>
        /// Process the cloud request. 
        /// </summary>
        /// <param name="xml">The XML to parse.</param>
        private void ProcessCloud(ReadXML xml)
        {
            int ch;

            while ((ch = xml.Read()) != -1)
            {
                if (ch == 0)
                {
                    if (xml.LastTag.Name.Equals("Header"))
                    {
                        this.headerProperties = xml.ReadPropertyBlock();
                    }
                    else if (xml.LastTag.Name.Equals("Session"))
                    {
                        this.sessionProperties = xml.ReadPropertyBlock();
                    }
                    else if (xml.LastTag.Name.Equals("Response"))
                    {
                        this.responseProperties = xml.ReadPropertyBlock();
                    }
                }
            }
        }

    }
}
#endif
