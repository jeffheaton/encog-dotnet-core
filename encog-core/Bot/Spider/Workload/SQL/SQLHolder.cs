// Encog Neural Network and Bot Library for DotNet v0.5
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
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
using System.Text;

namespace Encog.Bot.Spider.Workload.SQL
{
    /// <summary>
    /// SQLHolder: Holds the SQL commands for most databases.
    /// Some databases will require modifications of some of
    /// these commands.
    /// </summary>
    public class SQLHolder
    {
        /// <summary>
        /// SQL to clear the workload.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLClear()
        {
            return "DELETE FROM spider_workload";
        }

        /// <summary>
        /// SQL to clear the host table.  Leave any hosts marked to ignore.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLClear2()
        {
            return "DELETE FROM spider_host WHERE status <> 'I'";
        }

        /// <summary>
        /// Add to the workload.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLAdd()
        {
            return "INSERT INTO spider_workload(host,url,status,depth,url_hash,source_id) VALUES(?,?,?,?,?,?)";
        }

        /// <summary>
        /// Add to the host table.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLAdd2()
        {
            return "INSERT INTO spider_host(host,status,urls_done,urls_error) VALUES(?,?,?,?)";
        }

        /// <summary>
        /// Get the next workload item.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLGetWork()
        {
            return "SELECT workload_id,URL FROM spider_workload WHERE status =  ? AND host = ?";
        }

        /// <summary>
        /// Update a workload item just obtained.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLGetWork2()
        {
            return "UPDATE spider_workload SET status =  ? WHERE workload_id = ?";
        }

        /// <summary>
        /// Determine if the workload is empty.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLWorkloadEmpty()
        {
            return "SELECT COUNT(*) FROM spider_workload WHERE status in ('P','W') AND host =  ?";
        }

        /// <summary>
        /// Set the workload status of an item.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLSetWorkloadStatus()
        {
            return "UPDATE spider_workload SET status =  ? WHERE workload_id =  ?";
        }

        /// <summary>
        /// Set the host status of an item.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLSetWorkloadStatus2()
        {
            return "UPDATE spider_host SET urls_done =  urls_done + ?, urls_error =  urls_error + ? WHERE host =  ?";
        }

        /// <summary>
        /// Get the depth of a workload item.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLGetDepth()
        {
            return "SELECT url,depth FROM spider_workload WHERE url_hash =  ?";
        }

        /// <summary>
        /// Get the source of a workload item.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLGetSource()
        {
            return "SELECT w.url,s.url FROM spider_workload w,spider_workload s WHERE w.source_id =  s.workload_id AND w.url_hash =  ?";
        }


        /// <summary>
        /// Setup the workload for a resume.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLResume()
        {
            return "SELECT distinct host FROM spider_workload WHERE status =  'P'";
        }

        /// <summary>
        /// Setup the host table for a resume.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLResume2()
        {
            return "UPDATE spider_workload SET status =  'W' WHERE status =  'P'";
        }

        /// <summary>
        /// Get the ID of a workload item.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLGetWorkloadID()
        {
            return "SELECT workload_id,url FROM spider_workload WHERE url_hash =  ?";
        }

        /// <summary>
        /// Get the ID of a host.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLGetHostID()
        {
            return "SELECT host_id,host FROM spider_host WHERE host =  ?";
        }

        /// <summary>
        /// Get the next host to process.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLGetNextHost()
        {
            return "SELECT host_id,host FROM spider_host WHERE status =  ?";
        }

        /// <summary>
        /// Set the status of a host.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLSetHostStatus()
        {
            return "UPDATE spider_host SET status =  ? WHERE host_id =  ?";
        }

        /// <summary>
        /// Get the host that matches the specified ID.
        /// </summary>
        /// <returns>An SQL statement.</returns>
        public virtual String getSQLGetHost()
        {
            return "SELECT host FROM spider_host WHERE host_id =  ?";
        }


    }
}

