//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Data.Common;
using System.Data.OleDb;

namespace Encog.ML.Data.Buffer.CODEC
{
    /// <summary>
    /// A CODEC designed to work with SQL.
    /// </summary>
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

        /// <summary>
        /// Read a record.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="ideal">The ideal data.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Write a recrod.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="ideal">The ideal data.</param>
        public void Write(double[] input, double[] ideal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prepare to write.
        /// </summary>
        /// <param name="recordCount">The record count.</param>
        /// <param name="inputSize">The input size.</param>
        /// <param name="idealSize">The ideal size.</param>
        public void PrepareWrite(int recordCount, int inputSize, int idealSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prepare to read.
        /// </summary>
        public void PrepareRead()
        {
            results = statement.ExecuteReader();
        }

        /// <summary>
        /// The input size.
        /// </summary>
        public int InputSize
        {
            get { return inputSize; }
        }

        /// <summary>
        /// The ideal size.
        /// </summary>
        public int IdealSize
        {
            get { return idealSize; }
        }

        /// <summary>
        /// Close the codec.
        /// </summary>
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
