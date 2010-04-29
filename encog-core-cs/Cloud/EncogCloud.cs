using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Cloud
{
    public class EncogCloud
    {
        /**
	 * The default cloud server.
	 */
	public const String DEFAULT_SERVER = "http://cloud.encog.com/";

	/**
	 * The session.
	 */
	private String session;

	/**
	 * The server.
	 */
	private String server;

	/**
	 * Construct an Encog cloud connection. The connection will not be
	 * established until the connect method is called.
	 * 
	 * @param server
	 *            The server to connect to.
	 */
	public EncogCloud(String server) {
		this.server = server;
		if (!this.server.EndsWith("/")) {
			this.server += '/';
		}
	}

	/**
	 * Begin a task with the specified name.
	 * @param name The name of the task to begin.
	 * @return The new task.
	 */
	public CloudTask beginTask(String name) {
		CloudTask result = new CloudTask(this);
		result.init(name);

		return result;
	}

	/**
	 * Connect to the Encog cloud.
	 * @param uid The user id.
	 * @param pwd The password.
	 */
	public void connect(String uid, String pwd) {
		CloudRequest request = new CloudRequest();
		IDictionary<String, String> args = new Dictionary<String, String>();
        args["uid" ] = uid;
		args["pwd"] = pwd;
		request.performURLPOST(false, constructService("login"), args);
		if (!"success".Equals(request.getStatus())) {
			throw new EncogCloudError(request.getMessage());
		}
		this.session = request.getSession();
	}

	/**
	 * Construct a string that connects to the specified service.
	 * @param service The service to connect to.
	 * @return The complete URL.
	 */
	public String constructService(String service) {
		return this.server + service;
	}

	/**
	 * @return The session we are connected to.
	 */
	public String getSession() {
		return this.session;
	}

	/**
	 * @return True if we are connected.
	 */
	public bool isConnected() {
		return this.session != null;
	}

	/**
	 * Logout of the specified session.
	 */
	public void logout() {
		CloudRequest request = new CloudRequest();
		request.performURLGET(false, this.session + "logout");
		this.session = null;
	}

	/**
	 * Validate the session.
	 * @param failOnError True if an exception should be thrown on error.
	 */
	public void validateSession(bool failOnError) {
		int max;

		if (failOnError) {
			max = 1;
		} else {
			max = 5;
		}

		for (int i = 0; i < max; i++) {
			CloudRequest request = new CloudRequest();
			request.performURLGET(false, this.session);
			if ("success".Equals(request.getStatus())) {
				return;
			}
		}

		if (failOnError) {
			throw new EncogCloudError("Connection lost");
		}
	}
    }
}
