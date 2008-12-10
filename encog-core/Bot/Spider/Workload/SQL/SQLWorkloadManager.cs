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
using System.Threading;
using System.Data.Common;
using System.Data;
using System.Reflection;
using System.Data.OleDb;

using Encog.Bot.Spider.Logging;
using Encog.Util.DB;

namespace Encog.Bot.Spider.Workload.SQL
{
    /// <summary>
    /// SQLWorkloadManager: This workload manager stores the URL
    /// lists in an SQL database. This workload manager uses two
    /// tables, which can be created as follows:
    /// <pre>
    /// CREATE TABLE 'spider_host' (
    /// 'host_id' int(10) unsigned NOT NULL auto_increment,
    /// 'host' varchar(255) NOT NULL default '',
    /// 'status' varchar(1) NOT NULL default '',
    /// 'urls_done' int(11) NOT NULL,
    /// 'urls_error' int(11) NOT NULL,
    /// PRIMARY KEY  ('host_id')
    /// )
    ///
    /// CREATE TABLE 'spider_workload' (
    /// 'workload_id' int(10) unsigned NOT NULL auto_increment,
    /// 'host' int(10) unsigned NOT NULL,
    /// 'url' varchar(2083) NOT NULL default '',
    /// 'status' varchar(1) NOT NULL default '',
    /// 'depth' int(10) unsigned NOT NULL,
    /// 'url_hash' int(11) NOT NULL,
    /// 'source_id' int(11) NOT NULL,
    /// PRIMARY KEY  ('workload_id'),
    /// KEY 'status' ('status'),
    /// KEY 'url_hash' ('url_hash'),
    /// KEY 'host' ('host')
    /// )</pre>
    /// </summary>
    public class SQLWorkloadManager : IWorkloadManager
    {
        /// <summary>
        /// The database connection being used.
        /// </summary>
        public RepeatableConnection Connection
        {
            get
            {
                return connection;
            }
        }

        /// <summary>
        /// The spider being used.
        /// </summary>
        public Spider Spider
        {
            get
            {
                return spider;
            }
        }


        private SQLHolder holder = new SQLHolder();
        private RepeatableStatement stmtClear;
        private RepeatableStatement stmtClear2;
        private RepeatableStatement stmtAdd;
        private RepeatableStatement stmtAdd2;
        private RepeatableStatement stmtGetWork;
        private RepeatableStatement stmtGetWork2;
        private RepeatableStatement stmtWorkloadEmpty;
        private RepeatableStatement stmtSetWorkloadStatus;
        private RepeatableStatement stmtSetWorkloadStatus2;
        private RepeatableStatement stmtGetDepth;
        private RepeatableStatement stmtGetSource;
        private RepeatableStatement stmtResume;
        private RepeatableStatement stmtResume2;
        private RepeatableStatement stmtGetWorkloadID;
        private RepeatableStatement stmtGetHostID;
        private RepeatableStatement stmtGetNextHost;
        private RepeatableStatement stmtSetHostStatus;
        private RepeatableStatement stmtGetHost;

        /// <summary>
        /// The connection string for the connection.
        /// </summary>
        private String connectionString;

        /// <summary>
        /// Only one thread at a time is allowed to add to the
        /// workload.
        /// </summary>
        private Semaphore addLock;

        /// <summary>
        /// Is there any work?
        /// </summary>
        private AutoResetEvent workLatch;

        /// <summary>
        /// The maximum size a Uri can be.
        /// </summary>
        private int maxURLSize;

        /// <summary>
        /// The maximum size that a host can be.
        /// </summary>
        private int maxHostSize;

        /// <summary>
        /// All of the RepeatableStatement objects.
        /// </summary>
        private List<RepeatableStatement> statements = new List<RepeatableStatement>();

        /// <summary>
        /// Used to obtain the next URL.
        /// </summary>
        private RepeatableStatement.Results workResultSet = null;

        /// <summary>
        /// Used to obtain the next host.
        /// </summary>
        private RepeatableStatement.Results hostResultSet = null;

        /// <summary>
        /// A connection to a OLEDB database.
        /// </summary>
        private RepeatableConnection connection;

        /// <summary>
        /// The current host.
        /// </summary>
        private String currentHost;

        /// <summary>
        /// The ID of the current host.
        /// </summary>
        private int currentHostID = -1;

        /// <summary>
        /// The spider that this object is being used with.
        /// </summary>
        private Spider spider;


        /// <summary>
        /// Add the specified Uri to the workload.
        /// </summary>
        /// <param name="url">The URL to be added.</param>
        /// <param name="source">The page that contains this URL.</param>
        /// <param name="depth">The depth of this URL.</param>
        /// <returns>True if the URL was added, false otherwise.</returns>
        public virtual bool Add(Uri url, Uri source, int depth)
        {
            bool result = false;
            try
            {
                this.addLock.WaitOne();
                if (!Contains(url))
                {
                    String strURL = Truncate(url.ToString(), this.maxURLSize);
                    String strHost = Truncate(url.Host, this.maxHostSize).ToLower();
                    result = true;

                    // get the host
                    int hostID = GetHostID(url, false);

                    if (hostID == -1)
                    {
                        this.stmtAdd2.Execute(strHost, Status.STATUS_WAITING, 0, 0);
                        hostID = GetHostID(url, true);
                    }

                    // need to set the current host for the first time?
                    if (this.currentHostID == -1)
                    {
                        this.currentHostID = hostID;
                        this.currentHost = strHost;
                        this.stmtSetHostStatus.Execute(Status.STATUS_PROCESSING, this.currentHostID);
                    }

                    // now add workload element
                    if (source != null)
                    {
                        int sourceID = GetWorkloadID(source, true);
                        this.stmtAdd.Execute(hostID, strURL, Status.STATUS_WAITING, depth,
                            ComputeHash(url), sourceID);
                    }
                    else
                    {
                        this.stmtAdd.Execute(hostID, strURL, Status.STATUS_WAITING, depth,
                            ComputeHash(url), 0);
                    }

                    this.workLatch.Set();
                }

            }
            catch (Exception e)
            {
                throw (new WorkloadException(e));
            }
            finally
            {
                this.addLock.Release();
            }
            return result;
        }

        /// <summary>
        /// Clear the workload.
        /// </summary>
        public virtual void Clear()
        {
            this.stmtClear.Execute();
            this.stmtClear2.Execute();
        }

        /// <summary>
        /// Close the workload.
        /// </summary>
        public void Close()
        {
            if (this.workResultSet != null)
            {
                try
                {
                    this.workResultSet.Close();
                }
                catch (Exception)
                {
                    spider.Logging.Log(Logger.Level.ERROR,
                        "Error trying to close workload result set, ignoring...");
                }
                this.workResultSet = null;
            }

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
        /// Determine if the workload contains the specified URL.
        /// </summary>
        /// <param name="url">The Uri to search the workload for.</param>
        /// <returns>True if the workload contains the specified URL.</returns>
        public virtual bool Contains(Uri url)
        {
            try
            {
                return (GetWorkloadID(url, false) != -1);
            }
            catch (Exception e)
            {
                throw (new WorkloadException(e));
            }
        }

        /// <summary>
        /// Convert the specified String to a URL. If the string is
        /// too long or has other issues, throw a
        /// WorkloadException.
        /// </summary>
        /// <param name="url">A String to convert into a URL.</param>
        /// <returns>The URL.</returns>
        public virtual Uri ConvertURL(String url)
        {
            Uri result = null;

            url = url.Trim();
            if (url.Length > this.maxURLSize)
            {
                throw new WorkloadException("Uri size is too big, must be under "
                    + this.maxURLSize + " bytes.");
            }

            try
            {
                result = new Uri(url);
            }
            catch (UriFormatException e)
            {
                throw new WorkloadException(e);
            }
            return result;
        }

        /// <summary>
        /// Get the current host.
        /// </summary>
        /// <returns>The current host.</returns>
        public virtual String GetCurrentHost()
        {
            return this.currentHost;
        }

        /// <summary>
        /// Get the depth of the specified URL.
        /// </summary>
        /// <param name="url">The URL to get the depth of.</param>
        /// <returns>The depth of the specified URL.</returns>
        public virtual int GetDepth(Uri url)
        {
            RepeatableStatement.Results rs = null;
            try
            {
                rs = this.stmtGetDepth.ExecuteQuery(ComputeHash(url));
                while (rs.DataReader.Read())
                {
                    String u = (String)rs.DataReader[0];
                    if (u.Equals(url.ToString()))
                    {
                        return (int)rs.DataReader[1];
                    }
                }
                return 1;
            }
            catch (Exception e)
            {
                throw (new WorkloadException(e));
            }
            finally
            {
                if (rs != null)
                {
                    rs.Close();
                }
            }
        }

        /// <summary>
        /// Get the source page that contains the specified URL.
        /// </summary>
        /// <param name="url">The Uri to seek the source for.</param>
        /// <returns>The source of the specified URL.</returns>
        public virtual Uri GetSource(Uri url)
        {
            RepeatableStatement.Results rs = null;
            try
            {
                rs = this.stmtGetSource.ExecuteQuery(ComputeHash(url));
                while (rs.DataReader.Read())
                {
                    String u = (String)rs.DataReader[0];
                    if (u.Equals(url.ToString()))
                    {
                        return (new Uri((String)rs.DataReader[0]));
                    }
                }
                return null;
            }
            catch (UriFormatException e)
            {
                throw (new WorkloadException(e));
            }
            catch (Exception e)
            {
                throw (new WorkloadException(e));
            }
            finally
            {
                if (rs != null)
                {
                    rs.Close();
                }
            }
        }

        /// <summary>
        /// Get a new Uri to work on. Wait if there are no URL's
        /// currently available. Return null if done with the
        /// current host. The Uri being returned will be marked as
        /// in progress.
        /// </summary>
        /// <returns>The next URL to work on.</returns>
        public virtual Uri GetWork()
        {
            Uri url = null;
            do
            {
                url = GetWorkInternal();
                if (url == null)
                {
                    if (WorkloadEmpty())
                    {
                        break;
                    }

                }
            } while (url == null);

            return url;
        }

        /// <summary>
        /// Setup this workload manager for the specified spider.
        /// </summary>
        /// <param name="spider">The spider using this workload manager.</param>
        public virtual void Init(Spider spider)
        {
            this.spider = spider;
            this.addLock = new Semaphore(1, 1);
            this.workLatch = new AutoResetEvent(false);

            this.connection = new RepeatableConnection(spider.Options.DbConnectionString);
            this.connectionString = spider.Options.DbConnectionString;

            this.stmtClear = this.connection.CreateStatement(this.holder
                .getSQLClear());
            this.stmtClear2 = this.connection.CreateStatement(this.holder
                    .getSQLClear2());
            this.stmtAdd = this.connection.CreateStatement(this.holder.getSQLAdd());
            this.stmtAdd2 = this.connection.CreateStatement(this.holder
                    .getSQLAdd2());
            this.stmtGetWork = this.connection.CreateStatement(this.holder
                    .getSQLGetWork());
            this.stmtGetWork2 = this.connection.CreateStatement(this.holder
                    .getSQLGetWork2());
            this.stmtWorkloadEmpty = this.connection.CreateStatement(this.holder
                    .getSQLWorkloadEmpty());
            this.stmtSetWorkloadStatus = this.connection
                    .CreateStatement(this.holder.getSQLSetWorkloadStatus());
            this.stmtSetWorkloadStatus2 = this.connection
                    .CreateStatement(this.holder.getSQLSetWorkloadStatus2());
            this.stmtGetDepth = this.connection.CreateStatement(this.holder
                    .getSQLGetDepth());
            this.stmtGetSource = this.connection.CreateStatement(this.holder
                    .getSQLGetSource());
            this.stmtResume = this.connection.CreateStatement(this.holder
                    .getSQLResume());
            this.stmtResume2 = this.connection.CreateStatement(this.holder
                    .getSQLResume2());
            this.stmtGetWorkloadID = this.connection.CreateStatement(this.holder
                    .getSQLGetWorkloadID());
            this.stmtGetHostID = this.connection.CreateStatement(this.holder
                    .getSQLGetHostID());
            this.stmtGetNextHost = this.connection.CreateStatement(this.holder
                    .getSQLGetNextHost());
            this.stmtSetHostStatus = this.connection.CreateStatement(this.holder
                    .getSQLSetHostStatus());
            this.stmtGetHost = this.connection.CreateStatement(this.holder
                    .getSQLGetHost());

            try
            {
                this.connection.Open();

                this.maxURLSize = GetColumnSize("spider_workload", "url");
                this.maxHostSize = GetColumnSize("spider_host", "host");
            }
            catch (Exception e)
            {
                throw (new WorkloadException(e));
            }
        }

        /// <summary>
        /// Mark the specified Uri as error.
        /// </summary>
        /// <param name="url">The URL that had an error.</param>
        public virtual void MarkError(Uri url)
        {
            try
            {
                SetStatus(url, Status.STATUS_ERROR);
                this.workLatch.Set();
            }
            catch (Exception e)
            {
                throw (new WorkloadException(e));
            }
        }

        /// <summary>
        /// Mark the specified Uri as successfully processed.
        /// </summary>
        /// <param name="url">The Uri to mark as processed.</param>
        public virtual void MarkProcessed(Uri url)
        {
            try
            {
                SetStatus(url, Status.STATUS_DONE);
                this.workLatch.Set();
            }
            catch (Exception e)
            {
                throw (new WorkloadException(e));
            }
        }

        /// <summary>
        /// Move on to process the next host. This should only be
        /// called after getWork returns null.
        /// </summary>
        /// <returns>The name of the next host.</returns>
        public virtual String NextHost()
        {
            if (this.currentHostID == -1)
            {
                throw new WorkloadException(
                    "Attempting to obtain host before adding first URL.");
            }
            else
            {
                MarkHostProcessed(this.currentHost);
            }

            try
            {
                bool requery = false;

                if (this.hostResultSet == null)
                {
                    requery = true;
                }
                else
                {
                    if (!this.hostResultSet.DataReader.Read())
                    {
                        requery = true;
                    }
                }

                if (requery)
                {
                    if (this.hostResultSet != null)
                    {
                        this.hostResultSet.Close();
                    }

                    this.hostResultSet = this.stmtGetNextHost.ExecuteQuery(Status.STATUS_WAITING);

                    if (!this.hostResultSet.DataReader.Read())
                    {
                        return null;
                    }
                }

                this.currentHostID = (int)this.hostResultSet.DataReader[0];
                this.currentHost = (String)this.hostResultSet.DataReader[1];
                this.stmtSetHostStatus.Execute(Status.STATUS_PROCESSING, this.currentHostID);
                spider.Logging.Log(Logger.Level.INFO, "Moving to new host: " + this.currentHost);
                return this.currentHost;

            }
            catch (Exception e)
            {
                throw (new WorkloadException(e));
            }

        }

        /// <summary>
        /// Setup the workload so that it can be resumed from where
        /// the last spider left the workload.
        /// </summary>
        public virtual void Resume()
        {
            RepeatableStatement.Results rs = null;

            try
            {
                rs = this.stmtResume.ExecuteQuery();

                if (!rs.DataReader.Read())
                {
                    throw (new WorkloadException(
                        "Can't resume, unable to determine current host."));
                }

                this.currentHostID = (int)rs.DataReader[0];
                this.currentHost = (String)GetHost(this.currentHostID);
            }
            catch (Exception e)
            {
                throw (new WorkloadException(e));
            }
            finally
            {
                if (rs != null)
                {
                    rs.Close();
                }
            }

            this.stmtResume2.Execute();

        }

        /// <summary>
        /// If there is currently no work available, then wait
        /// until a new Uri has been added to the workload.
        /// </summary>
        /// <param name="time">The amount of time to wait.</param>
        public virtual void WaitForWork(int time)
        {

            this.workLatch.WaitOne();


        }

        /// <summary>
        /// Return true if there are no more workload units.
        /// </summary>
        /// <returns>Returns true if there are no more workload units.</returns>
        public virtual bool WorkloadEmpty()
        {
            RepeatableStatement.Results rs = null;

            try
            {
                rs = this.stmtWorkloadEmpty.ExecuteQuery(this.currentHostID);
                if (!rs.DataReader.Read())
                {
                    return true;
                }
                return ((int)rs.DataReader[0] < 1);
            }
            catch (Exception e)
            {
                throw (new WorkloadException(e));
            }
            finally
            {
                if (rs != null)
                {
                    rs.Close();
                }
            }
        }

        /// <summary>
        /// Compute a hash for a URL.
        /// </summary>
        /// <param name="url">The Uri to compute the hash for.</param>
        /// <returns>The hash code.</returns>
        protected virtual int ComputeHash(Uri url)
        {
            String str = url.ToString().Trim();

            int result = str.GetHashCode();
            result = (result % 0xffff);
            return result;
        }

        /// <summary>
        /// Return the size of the specified column.
        /// </summary>
        /// <param name="table">The table that contains the column.</param>
        /// <param name="column">The column to get the size for.</param>
        /// <returns>The size of the column.</returns>
        public virtual int GetColumnSize(String table, String column)
        {
            try
            {
                DataTable datatable = connection.Connection.GetSchema("Columns");
                // Unfortunatly the hard coded string values below are not really 
                // standardized, even amoung Microsoft products.  If you find that
                // this method is always returning -1 for the column length, try
                // the following lines of code, they will dump all information 
                // from the schema to the console.  Then you can find out what 
                // your database/driver uses for column_length. 

                //foreach (DataRow row in datatable1.Rows)
                //{
                //    foreach (DataColumn col in datatable.Columns)
                //    {
                //        Console.WriteLine(col.ToString() + " = " + row[col].ToString());
                //    }
                //}

                foreach (System.Data.DataRow row in datatable.Rows)
                {
                
                    if (String.Compare(row["COLUMN_NAME"].ToString(), column, true) == 0)
                    {
                        return int.Parse(row["CHARACTER_MAXIMUM_LENGTH"].ToString());
                    }
                }

                
            }
            catch (Exception)
            {
                // Like was stated above, different databases seem to name things
                // differently.  If an error occurs, then just report that we can't
                // get the length, by falling through to the -1.
            }

            return 1;
        }

        /// <summary>
        /// Get the host name associated with the specified host id.
        /// </summary>
        /// <param name="hostID">The host id to look up.</param>
        /// <returns>The name of the host.</returns>
        protected virtual String GetHost(int hostID)
        {
            RepeatableStatement.Results rs = null;

            try
            {
                rs = this.stmtGetHost.ExecuteQuery(hostID);
                if (!rs.DataReader.Read())
                {
                    throw new WorkloadException("Can't find previously created host.");
                }
                return (String)rs.DataReader[0];
            }
            catch (Exception e)
            {
                throw new WorkloadException(e);
            }
            finally
            {
                if (rs != null)
                {
                    rs.Close();
                }
            }
        }

        /// <summary>
        /// Get the id for the specified host name.
        /// </summary>
        /// <param name="host">The host to lookup.</param>
        /// <param name="require">Should an exception be thrown if the host is not located.</param>
        /// <returns>The id of the specified host name.</returns>
        protected virtual int GetHostID(String host, bool require)
        {
            RepeatableStatement.Results rs = null;

            // is this the current host?
            if (this.currentHostID != -1)
            {
                if (String.Compare(this.currentHost, host, true) == 0)
                {
                    return this.currentHostID;
                }
            }

            // use the database to find it
            try
            {
                rs = this.stmtGetHostID.ExecuteQuery(host);

                if (rs.DataReader.Read())
                {
                    return (int)rs.DataReader[0];
                }

            }
            finally
            {
                if (rs != null)
                {
                    rs.Close();
                }
            }

            if (require)
            {
                StringBuilder str = new StringBuilder();
                str.Append("Failed to find previously visited Host,");
                str.Append("Host=\"");
                str.Append(host);
                str.Append("\".");
                throw (new WorkloadException(str.ToString()));
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Get the ID for the given host. The host name is
        /// extracted from the specified URL.
        /// </summary>
        /// <param name="url">The Uri that specifies the host name to lookup.</param>
        /// <param name="require">Should an exception be thrown if the host is not located.</param>
        /// <returns></returns>
        protected virtual int GetHostID(Uri url, bool require)
        {
            String host = url.Host.ToLower();
            return GetHostID(host, require);
        }

        /// <summary>
        /// Called internally to get a work unit. This function
        /// does not wait for work, rather it simply returns null.
        /// </summary>
        /// <returns>The next Uri to process.</returns>
        protected virtual Uri GetWorkInternal()
        {
            if (this.currentHostID == -1)
            {
                throw new WorkloadException(
                    "Attempting to obtain work before adding first URL.");
            }

            try
            {
                bool requery = false;

                if (this.workResultSet == null)
                {
                    requery = true;
                }
                else
                {
                    if (!this.workResultSet.DataReader.Read())
                    {
                        requery = true;
                    }
                }

                if (requery)
                {
                    if (this.workResultSet != null)
                    {
                        this.workResultSet.Close();
                    }

                    this.workResultSet = this.stmtGetWork.ExecuteQuery(Status.STATUS_WAITING,
                        this.currentHostID);

                    if (!this.workResultSet.DataReader.Read())
                    {
                        return null;
                    }
                }

                int id = (int)this.workResultSet.DataReader[0];
                String url = (String)this.workResultSet.DataReader[1];

                this.stmtGetWork2.Execute(Status.STATUS_PROCESSING, id);
                return new Uri(url);

            }
            catch (UriFormatException e)
            {
                throw (new WorkloadException(e));
            }
            catch (Exception e)
            {
                throw (new WorkloadException(e));
            }
        }

        /// <summary>
        /// Get the workload ID, given a URL.
        /// </summary>
        /// <param name="url"> The URL to look up.</param>
        /// <param name="require">Should an exception be thrown if the workload
        /// is not located.</param>
        /// <returns>The ID of the workload.</returns>
        protected virtual int GetWorkloadID(Uri url, bool require)
        {
            int hash = 0;
            RepeatableStatement.Results rs = null;
            try
            {
                hash = ComputeHash(url);
                rs = this.stmtGetWorkloadID.ExecuteQuery(hash);
                while (rs.DataReader.Read())
                {
                    if (rs.DataReader[1].Equals(url.ToString()))
                    {
                        return (int)rs.DataReader[0];
                    }
                }
            }
            finally
            {
                if (rs != null)
                {
                    rs.Close();
                }
            }

            if (require)
            {
                StringBuilder str = new StringBuilder();
                str.Append("Failed to find previously visited URL, hash=\"");
                str.Append(hash);
                str.Append("\", URL=\"");
                str.Append(url.ToString());
                str.Append("\".");
                throw (new WorkloadException(str.ToString()));
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Mark the specified host as processed.
        /// </summary>
        /// <param name="host">The host to mark.</param>
        protected virtual void MarkHostProcessed(String host)
        {
            try
            {
                int hostID = this.GetHostID(host, true);
                this.stmtSetHostStatus.Execute(Status.STATUS_DONE, hostID);
            }
            catch (Exception e)
            {
                throw new WorkloadException(e);
            }
        }

        /// <summary>
        /// Set the status for the specified URL.
        /// </summary>
        /// <param name="url">The Uri to set the status for.</param>
        /// <param name="status">What to set the status to.</param>
        protected virtual void SetStatus(Uri url, String status)
        {
            int id = GetWorkloadID(url, true);
            this.stmtSetWorkloadStatus.Execute("" + status, id);
            if (String.Compare(status, Status.STATUS_ERROR, true) == 0)
            {
                this.stmtSetWorkloadStatus2.Execute(0, 1, url.Host.ToLower());
            }
            else if (String.Compare(status, Status.STATUS_DONE, true) == 0)
            {
                this.stmtSetWorkloadStatus2.Execute(1, 0, url.Host.ToLower());
            }

        }

        /// <summary>
        /// Truncate a string to the specified length.
        /// </summary>
        /// <param name="str">The string to truncate.</param>
        /// <param name="length">The length to truncate the string to.</param>
        /// <returns>The truncated string.</returns>
        protected virtual String Truncate(String str, int length)
        {
            if (str.Length < length)
            {
                return str;
            }
            else
            {
                return str.Substring(0, length);
            }
        }

        /// <summary>
        /// Try to open the database connection.
        /// </summary>
        public virtual void TryOpen()
        {
            Exception ex = null;

            this.spider.Logging.Log(Logger.Level.ERROR,
                "Lost connection to database, trying to reconnect.");

            for (int i = 1; i < 120; i++)
            {
                try
                {
                    Close();
                }
                catch (Exception e1)
                {
                    this.spider.Logging.Log(Logger.Level.ERROR,
                      "Failed while trying to close lost connection, ignoring...", e1);
                }

                ex = null;

                try
                {
                    this.spider.Logging.Log(Logger.Level.ERROR, "Attempting database reconnect");
                    connection.Open();
                    this.spider.Logging.Log(Logger.Level.ERROR, "Database connection reestablished");
                    break;
                }
                catch (Exception e)
                {
                    ex = e;
                    this.spider.Logging.Log(Logger.Level.ERROR, "Reconnect failed", ex);
                }

                if (ex != null)
                {

                    this.spider.Logging.Log(Logger.Level.ERROR, "Reconnect attempt " + i
                      + " failed.  Waiting to try again.");
                    Thread.Sleep(30000);

                }
            }

            if (ex != null)
            {
                throw (new WorkloadException(ex));
            }

        }

        /// <summary>
        /// Create the correct type of SQLHolder to go along with this workload
        /// manager.
        /// </summary>
        /// <returns>A SQLHolder.</returns>
        public virtual SQLHolder CreateSQLHolder()
        {
            return new SQLHolder();
        }
    }
}
