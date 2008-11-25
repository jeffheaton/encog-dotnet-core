using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Threading;

namespace Encog.Util.DB
{
    public class RepeatableStatement
    {
        /// <summary>
        /// Simple internal class that holds the ResultSet from a
        /// query.
        /// </summary>
        public class Results
        {
            public DbDataReader DataReader
            {
                get
                {
                    return this.dataReader;
                }
            }

            /// <summary>
            /// The DbCommand that generated these results.
            /// </summary>
            private DbCommand command;

            /// <summary>
            /// The DbDataReader that was generated.
            /// </summary>
            private DbDataReader dataReader;

            /// <summary>
            /// Construct a Results object.
            /// </summary>
            /// <param name="command">The DbCommand for these results.</param>
            /// <param name="resultSet">The DbDataReader for these results.</param>
            public Results(DbCommand command, DbDataReader resultSet)
            {
                this.command = command;
                this.dataReader = resultSet;
            }

            /// <summary>
            /// Close these results.
            /// </summary>
            public void Close()
            {
                this.dataReader.Close();
            }
        }

        /// <summary>
        /// The manager for this RepeatableStatement.
        /// </summary>
        private RepeatableConnection manager;

        /// <summary>
        /// The SQL for this RepeatableStatement.
        /// </summary>
        private String sql;


        /// <summary>
        /// The PreparedStatements that are assigned to each
        /// thread.
        /// </summary>
        private IList<DbCommand> statementCache = new List<DbCommand>();

        /// <summary>
        /// Construct a repeatable statement based on the
        /// specified SQL 
        /// </summary>
        /// <param name="sql">he SQL to base this statement
        /// </param>
        public RepeatableStatement(String sql)
        {
            this.sql = sql;
        }

        /// <summary>
        /// Close the statement.
        /// </summary>
        public void Close()
        {
            foreach (DbCommand statement in this.statementCache)
            {
                statement.Dispose();
            }
        }

        /// <summary>
        /// Create the statement, so that it is ready to assign
        /// DbCommand objects.
        /// </summary>
        /// <param name="manager"></param>
        public void Init(RepeatableConnection manager)
        {
            Close();
            this.manager = manager;
        }

        /// <summary>
        /// Execute SQL that does not return a result set. If an
        /// error occurs, the statement will be retried until it is
        /// successful. This handles broken connections.
        /// </summary>
        /// <param name="parameters">The parameters for this SQL.</param>
        public void Execute(params Object[] parameters)
        {
            DbCommand statement = null;

            try
            {
                statement = ObtainStatement();

                for (; ; )
                {
                    try
                    {
                        statement.Parameters.Clear();
                        foreach (Object parameter in parameters)
                        {
                            DbParameter dbParam = statement.CreateParameter();

                            if (parameter == null)
                            {
                                dbParam.Value = DBNull.Value;
                            }
                            else
                            {
                                dbParam.Value = parameter;
                            }
                            statement.Parameters.Add(dbParam);
                        }

                        statement.ExecuteNonQuery();
                        return;
                    }
                    catch (Exception)
                    {
                        this.manager.TryOpen();
                    }
                }
            }
            catch (Exception e)
            {
                throw (new DBError(e));
            }
            finally
            {
                if (statement != null)
                {
                    ReleaseStatement(statement);
                }
            }
        }


        /// <summary>
        /// Execute an SQL query that returns a result set. If an
        /// error occurs, the statement will be retried until it is
        /// successful. This handles broken connections.
        /// </summary>
        /// <param name="parameters">The parameters for this SQL.</param>
        /// <returns>The results of the query.</returns>
        public Results ExecuteQuery(params Object[] parameters)
        {
            DbCommand statement = null;
            try
            {
                statement = ObtainStatement();

                for (; ; )
                {
                    try
                    {
                        statement.Parameters.Clear();
                        foreach (Object parameter in parameters)
                        {
                            DbParameter dbParam = statement.CreateParameter();

                            if (parameter == null)
                            {
                                dbParam.Value = DBNull.Value;
                            }
                            else
                            {
                                dbParam.Value = parameter;
                            }
                            statement.Parameters.Add(dbParam);
                        }

                        DbDataReader reader = statement.ExecuteReader();
                        Results results = new Results(statement, reader);
                        return results;
                    }
                    catch (Exception)
                    {
                        this.manager.TryOpen();
                    }
                }
            }
            catch (Exception e)
            {
                throw (new DBError(e));
            }

        }

        /// <summary>
        /// Obtain a statement. Each thread should use their own
        /// statement, and then call the releaseStatement method
        /// when they are done.
        /// </summary>
        /// <returns>A PreparedStatement object.</returns>
        private DbCommand ObtainStatement()
        {
            DbCommand result = null;

            try
            {
                Monitor.Enter(this);
                //if (this.statementCache.Count == 0)
                {
                    result = this.manager.Connection.CreateCommand();
                    result.CommandText = this.sql;
                    result.Prepare();
                    result.Connection = this.manager.Connection;
                }
                /*else
                {
                    result = this.statementCache[0];
                    this.statementCache.Remove(result);
                    result.Parameters.Clear();
                    result.Connection = this.manager.Connection;
                }*/

            }
            finally
            {
                Monitor.Exit(this);
            }

            return result;
        }


        /**
         * This method releases statements after the thread is
         * done with them. These statements are not closed, but
         * rather cached until another thread has need of them.
         * 
         * @param stmt
         *          The statement that is to be released.
         */
        private void ReleaseStatement(DbCommand stmt)
        {
            stmt.Dispose();
            //this.statementCache.Add(stmt);
        }

    }
}
