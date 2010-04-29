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
    public class CloudRequest
    {
        
	/**
	 * The header properties.
	 */
	private IDictionary<String, String> headerProperties = new Dictionary<String, String>();
	
	/**
	 * The session properties.
	 */
	private IDictionary<String, String> sessionProperties = new Dictionary<String, String>();
	
	/**
	 * The response properties.
	 */
	private IDictionary<String, String> responseProperties = new Dictionary<String, String>();

	/**
	 * @return The message returned from the cloud.
	 */
	public String getMessage() {
        if (!this.headerProperties.ContainsKey("message"))
            return null;
        else
		    return this.headerProperties["message"];
	}

	/**
	 * Get a response property.
	 * @param key The key.
	 * @return The property.
	 */
	public String getResponseProperty(String key) {
        if (!this.responseProperties.ContainsKey(key))
            return null;
        else
		    return this.responseProperties[key];
	}

	/**
	 * @return The service.
	 */
	public String getService() {
        if (this.headerProperties.ContainsKey("service"))
            return this.headerProperties["service"];
        else
            return null;
	}

	/**
	 * @return The url.
	 */
	public String getSession() {
        if (this.sessionProperties.ContainsKey("url"))
            return this.sessionProperties["url"];
        else
            return null;
	}

	/**
	 * @return The status.
	 */
	public String getStatus() {
        if (this.headerProperties.ContainsKey("status"))
            return this.headerProperties["status"];
        else
            return null;
	}

	/**
	 * Handle the cloud response.
	 * @param contents The contents.
	 */
	private void handleResponse(String contents) {
        byte[] bin = System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(contents);
		MemoryStream istream = new MemoryStream(bin);
		ReadXML xml = new ReadXML(istream);
		int ch;

		while ((ch = xml.Read()) != -1) {
			if (ch == 0) {
				if (xml.LastTag.Name.Equals("EncogCloud")) {
					processCloud(xml);
				}
			}
		}

		if ((getStatus() == null) || getStatus().Equals("failed")) {
			throw new EncogCloudError(getMessage());
		}
	}

	/**
	 * Perform a GET request.
	 * @param async True if this request should be asynchronous.
	 * @param url The URL.
	 */
	public void performURLGET(bool async, String url) {
		try {
			if (async) {
				AsynchronousCloudRequest request = new AsynchronousCloudRequest(url);
				Thread t = new Thread(new ThreadStart(request.run));
				t.Start();
			} else {
				String contents = BotUtil.LoadPage(new Uri(url));
				handleResponse(contents);
			}
		} catch ( IOException e) {
			throw new EncogCloudError(e);
		}
	}

	/**
	 * Perform a POST to the cloud.
	 * @param async True if this request should be asynchronous.
	 * @param service The service.
	 * @param args The POST arguments.
	 */
	public void performURLPOST(bool async, String service,
			IDictionary<String, String> args) {
		try {
			if (async) {
				AsynchronousCloudRequest request = new AsynchronousCloudRequest(
						service, args);
				Thread t = new Thread(new ThreadStart(request.run));
				t.Start();
			} else {
                String contents = BotUtil.POSTPage(new Uri(service), args);
				handleResponse(contents);
			}
		} catch (IOException e) {
			throw new EncogCloudError(e);
		}
	}

	/**
	 * Process the cloud request.
	 * @param xml The XML to parse.
	 */
	private void processCloud(ReadXML xml) {
		int ch;

		while ((ch = xml.Read()) != -1) {
			if (ch == 0) {
				if (xml.LastTag.Name.Equals("Header")) {
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
