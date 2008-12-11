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
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.OleDb;
using System.Threading;

namespace Encog.Util.DB
{
    /// <summary>
    /// RepeatableConnection is used to create RepeatableStatement objects.
    /// </summary>
    public class RepeatableConnection
    {
        /// <summary>
        /// The connection string that was used.
        /// </summary>
        public String ConnectionString
        {
            get
            {
                return this.connectionString;
            }
        }

        /// <summary>
        /// The connection that is being used.
        /// </summary>
        public DbConnection Connection
        {
            get
            {
                return this.connection;
            }
        }

        /// <summary>
        /// The statements that have been created.
        /// </summary>
        public IList<RepeatableStatement> Statements
        {
            get
            {
                return this.statements;
            }
        }
        
        /// <summary>
        /// The cconnection string for the connection.
        /// </summary>
        private String connectionString;

        /// <summary>
        /// The connection.
        /// </summary>
        private DbConnection connection;

        /// <summary>
        /// The statements that have been created.
        /// </summary>
        private IList<RepeatableStatement> statements = new List<RepeatableStatement>();


        /// <summary>
        /// The database connection.
        /// </summary>
        public DbConnection DBConnection
        {
            get
            {
                return this.connection;
            }
        }


        /// <summary>
        /// Construct a connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public RepeatableConnection(String connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        public void Close()
        {
            foreach (RepeatableStatement statement in this.statements)
            {
                statement.Close();
            }

            if (this.connection != null)
            {
                this.connection.Close();
            }

        }

        /// <summary>
        /// Create the specified statement.
        /// </summary>
        /// <param name="sql">The SQL to create the statement from.</param>
        /// <returns>The statement that was created.</returns>
        public RepeatableStatement CreateStatement(String sql)
        {
            RepeatableStatement result;
            this.statements.Add(result = new RepeatableStatement(sql));
            return result;
        }



        /// <summary>
        /// Open a database connection.
        /// </summary>
        public void Open()
        {
            connection = new OleDbConnection(this.connectionString);
            connection.Open();

            foreach (RepeatableStatement statement in this.statements)
            {
                statement.Init(this);
            }
        }

        /// <summary>
        /// Try to open the database connection.
        /// </summary>
        public void TryOpen()
        {
            Exception ex = null;

            for (int i = 1; i < 120; i++)
            {
                try
                {
                    Close();
                }
                catch (Exception)
                {
                }

                ex = null;

                try
                {
                    Open();
                    break;
                }
                catch (Exception e)
                {
                    ex = e;
                }


                Thread.Sleep(30000);

            }

            if (ex != null)
            {
                throw new DBError(ex);
            }

        }
    }
}
