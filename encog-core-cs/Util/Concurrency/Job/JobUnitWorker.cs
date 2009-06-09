using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Concurrency.Job
{
    public class JobUnitWorker : IEncogTask {
	
	private JobUnitContext context;
	
	public JobUnitWorker(JobUnitContext context)
	{
		this.context = context;
	}
	
	public void Run()
	{
		this.context.Owner.PerformJobUnit(this.context);
	}
}

}
