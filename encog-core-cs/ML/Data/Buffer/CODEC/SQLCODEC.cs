using System;
using System.Data.Common;
using System.Data.OleDb;

namespace Encog.ML.Data.Buffer.CODEC
{
    public class SQLCODEC : IDataSetCODEC
    {
                /// <summary>
        /// What is the size of the input data?
        /// </summary>
        private int inputSize;

        /// <summary>
        /// What is the size of the ideal data?
        /// </summary>
        private int idealSize;

        /// <summary>
        /// The database connection.
        /// </summary>
        private DbConnection connection;

        /// <summary>
        /// The SQL statement being used.
        /// </summary>
        private DbCommand statement;

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
            this.connection = new OleDbConnection(connectString);
            this.connection.Open();
            this.statement = connection.CreateCommand();
            this.statement.CommandText = sql;
            this.statement.Prepare();
            this.statement.Connection = this.connection;
        }

        public bool Read(double[] input, double[] ideal)
        {
            if (!this.results.NextResult())
                return false;

            for (int i = 1; i <= this.inputSize; i++)
            {
                input[i - 1] = this.results.GetDouble(i);
            }

            if (this.idealSize > 0)
            {

                for (int i = 1; i <= this.idealSize; i++)
                {
                    ideal[i - 1] =
                        this.results.GetDouble(i + this.inputSize);
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
            this.results = this.statement.ExecuteReader();
        }

        public int InputSize
        {
            get { return this.inputSize; }
        }

        public int IdealSize
        {
            get { return this.idealSize; }
        }

        public void Close()
        {
            if (this.connection != null)
            {
                this.connection.Close();
            }
            if (this.results != null)
            {
                this.results.Close();
            }
        }
    }
}
