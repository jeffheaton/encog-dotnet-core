#if !SILVERLIGHT
using System;
using System.Data.Common;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Buffer;
using Encog.ML.Data.Buffer.CODEC;

namespace Encog.ML.Data.Specific
{
    /// <summary>
    /// A dataset based on a SQL query. This is not a memory based dataset, so it can
    /// handle very large datasets without a memory issue. and can handle very large
    /// datasets.
    /// </summary>
    public class SQLMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// The database connection.
        /// </summary>
        private DbConnection connection;

        /// <summary>
        /// What is the size of the ideal data?
        /// </summary>
        private int idealSize;

        /// <summary>
        /// What is the size of the input data?
        /// </summary>
        private int inputSize;

        /// <summary>
        /// The SQL statement being used.
        /// </summary>
        private DbCommand statement;

        /// <summary>
        /// Create a SQL neural data set.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="inputSize">The size of the input data being read.</param>
        /// <param name="idealSize">The size of the ideal output data being read.</param>
        /// <param name="connectString">The connection string.</param>
        public SQLMLDataSet(String sql, int inputSize,
                            int idealSize, String connectString)
        {
            IDataSetCODEC codec = new SQLCODEC(sql, inputSize, idealSize, connectString);
            var load = new MemoryDataLoader(codec);
            load.Result = this;
            load.External2Memory();
        }
    }
}

#endif