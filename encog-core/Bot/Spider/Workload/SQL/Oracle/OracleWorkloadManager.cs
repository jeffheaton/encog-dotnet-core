// Encog Neural Network and Bot Library v1.x (DotNet)
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
using System.Data.Common;
using System.Data;

namespace Encog.Bot.Spider.Workload.SQL.Oracle
{
    /// <summary>
    /// OracleWorkloadManager: Contains a workload manager 
    /// customized to Oracle.
    /// </summary>
    class OracleWorkloadManager : SQLWorkloadManager
    {
        /// <summary>
        /// Creates an instance of the OracleHolder class.
        /// </summary>
        /// <returns>An SQLHolder derived object.</returns>
        public override SQLHolder CreateSQLHolder()
        {
            return new OracleHolder();
        }


        /// <summary>
        /// Return the size of the specified column.  Oracle
        /// requires the "%" parameter to be specified.
        /// </summary>
        /// <param name="table">The table that contains the column.</param>
        /// <param name="column">The column to get the size for.</param>
        /// <returns>The size of the column.</returns>
        public override int GetColumnSize(String table, String column)
        {
            String[] restriction = { null, null, table, "%" };
            DataTable dt = this.Connection.Connection.GetSchema("Columns", restriction);
            foreach (System.Data.DataRow row in dt.Rows)
            {

                if (String.Compare(row["column_name"].ToString(), column, true) == 0)
                {
                    return int.Parse(row["column_size"].ToString());
                }
            }

            return -1;
        }
    }
}
