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
using Encog.Bot.Spider.Workload.SQL;

namespace Encog.Bot.Spider.Workload.SQL.Oracle
{
    /// <summary>
    /// OracleHolder: Contains SQL statements customized to Oracle.
    /// </summary>
    class OracleHolder:SQLHolder
    {
        public override String getSQLAdd()
        {
            return "INSERT INTO spider_workload(workload_id,host,url,status,depth,url_hash,source_id) VALUES(spider_workload_seq.NEXTVAL,?,?,?,?,?,?)";
        }

        public override String getSQLAdd2()
        {
            return "INSERT INTO spider_host(host_id,host,status,urls_done,urls_error) VALUES(spider_host_seq.NEXTVAL,?,?,?,?)";
        }
    }
}
