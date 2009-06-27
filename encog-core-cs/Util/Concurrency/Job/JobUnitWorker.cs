// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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
