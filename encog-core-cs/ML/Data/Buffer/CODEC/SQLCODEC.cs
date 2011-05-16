using System;
using System.Data.Common;
using System.Data.OleDb;

namespace Encog.ML.Data.Buffer.CODEC
{
    public class SQLCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The database connection.
        /// </summary>
        private readonly DbConnection connection;

        /// <summary>
        /// What is the size of the ideal data?
        /// </summary>
        private readonly int idealSize;

        /// <summary>
        /// What is the size of the input data?
        /// </summary>
        private readonly int inputSize;

        /// <summary>
        /// The SQL statement being used.
        /// </summary>
        private readonly DbCommand statement;

        /// <summary>
        /// Holds results from the SQL query.
        /// </summary>
        private DbDataReader results;

        /// <summary>
        /// Create a SQL neural data set.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="inputSize">The size of the input data being read.</param>
        /// <param name="idealSize">The size of the ideal output data being read.</param>
        /// <param name="connectString">The connection string.</param>
        public SQLCODEC(String sql, int inputSize,
                        int idealSize, String connectString)
        {
            this.inputSize = inputSize;
            this.idealSize = idealSize;
            connection = new OleDbConnection(connectString);
            connection.Open();
            statement = connection.CreateCommand();
            statement.CommandText = sql;
            statement.Prepare();
            statement.Connection = connection;
        }

        #region IDataSetCODEC Members

        public bool Read(double[] input, double[] ideal)
        {
            if (!results.NextResult())
                return false;

            for (int i = 1; i <= inputSize; i++)
            {
                input[i - 1] = results.GetDouble(i);
            }

            if (idealSize > 0)
            {
                for (int i = 1; i <= idealSize; i++)
                {
                    ideal[i - 1] =
                        results.GetDouble(i + inputSize);
                }
            }

            return true;
        }

        public void Write(double[] input, double[] ideal)
        {
            throw new NotImplementedException();
        }

        public void PrepareWrite(int recordCount, int inputSize, int idealSize)
        {
            throw new NotImplementedException();
        }

        public void PrepareRead()
        {
            results = statement.ExecuteReader();
        }

        public int InputSize
        {
            get { return inputSize; }
        }

        public int IdealSize
        {
            get { return idealSize; }
        }

        public void Close()
        {
            if (connection != null)
            {
                connection.Close();
            }
            if (results != null)
            {
                results.Close();
            }
        }

        #endregion
    }
}