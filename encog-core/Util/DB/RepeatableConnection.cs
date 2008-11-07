using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.OleDb;
using System.Threading;

namespace Encog.Util.DB
{
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


        public DbConnection DBConnection
        {
            get
            {
                return this.connection;
            }
        }



        public RepeatableConnection(String connectionString)
        {
            this.connectionString = connectionString;
        }

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
