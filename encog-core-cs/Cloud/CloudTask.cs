using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Cloud
{
    public class CloudTask
    {
        	/**
	 * The cloud service.
	 */
	private EncogCloud cloud;
	
	/**
	 * The URL for this task.
	 */
	private String taskURL;

	/**
	 * Construct a cloud task.
	 * @param cloud The cloud this task belongs to.
	 */
	public CloudTask(EncogCloud cloud) {
		this.cloud = cloud;
	}

	/**
	 * @return The cloud that this task belongs to.
	 */
	public EncogCloud getCloud() {
		return this.cloud;
	}

	/**
	 * Setup this task.
	 * @param name The name of this task.
	 */
	public void init(String name) {

		if (this.cloud.getSession() == null) {
			throw new EncogCloudError(
					"Session must be established before a task is created.");
		}

		CloudRequest request = new CloudRequest();
		String url = this.cloud.getSession();
		url += "task/create";
		IDictionary<String, String> args = new Dictionary<String, String>();
		args["name"] = name;
		args["status"] = "Starting...";
		request.performURLPOST(false, url, args);
		this.taskURL = this.cloud.getSession() + "task/"
				+ request.getResponseProperty("id") + "/";
	}

	/**
	 * Set the status for this task.
	 * @param status The status for this task.
	 */
	public void setStatus(String status) {
		if (this.taskURL == null) {
			throw new EncogCloudError("Can't set status for inactive task.");
		}

		CloudRequest request = new CloudRequest();
		String url = this.taskURL + "update";

		IDictionary<String, String> args = new Dictionary<String, String>();
		args["status"] = status;
		request.performURLPOST(true, url, args);

	}

	/**
	 * Stop this task.
	 * @param finalStatus The final status for this task.
	 */
	public void stop(String finalStatus) {
		if (this.taskURL == null) {
			throw new EncogCloudError("Can't stop inactive task.");
		}

		// send final status
		CloudRequest request = new CloudRequest();
		String url = this.taskURL + "update";

		IDictionary<String, String> args = new Dictionary<String, String>();
		args["status"] = finalStatus;
		request.performURLPOST(false, url, args);

		// stop
		url = this.taskURL + "stop";
		request = new CloudRequest();
		request.performURLGET(false, url);
	}
    }
}
