using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Concurrency.Job
{
    /// <summary>
    /// A worker job for Encog concurrency
    /// </summary>
    public class JobUnitWorker : IEncogTask {
	
    /// <summary>
    /// The context for this job unit.
    /// </summary>
	private JobUnitContext context;
	
    /// <summary>
    /// Construct a job unit worker to execute the specified job.
    /// </summary>
    /// <param name="context">The context of the job to execute.</param>
	public JobUnitWorker(JobUnitContext context)
	{
		this.context = context;
	}
	
    /// <summary>
    /// Run this job.
    /// </summary>
	public void Run()
	{
		this.context.Owner.PerformJobUnit(this.context);
	}
}

}
