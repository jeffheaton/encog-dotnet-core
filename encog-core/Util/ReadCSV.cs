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
using System.IO;

namespace Encog.Util
{
    /// <summary>
    /// ReadCSV: Read and parse CSV format files.
    /// </summary>
    public class ReadCSV
    {
        /// <summary>
        /// The format that dates are expected to be in.
        /// </summary>
        public const String dateFormat = "yyyy-MM-dd";

        /// <summary>
        /// The file to read.
        /// </summary>
        private TextReader reader;

        /// <summary>
        /// The names of all of the columns, read from the first line of the file.
        /// </summary>
        private IDictionary<String, int> columns = new Dictionary<String, int>();

        /// <summary>
        /// The data for the current line.
        /// </summary>
        private String[] data;

        private char delim;
        private bool headers;

        /// <summary>
        /// Format a date/time object to the same format that we parse in.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>A formatted date and time.</returns>
        public static String DisplayDate(DateTime date)
        {
            return date.ToString(dateFormat);
        }

        /// <summary>
        /// The number of columns.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                if (this.data == null)
                    return 0;
                else
                    return this.data.Length;
            }
        }

        /// <summary>
        /// Parse a date using the specified format.
        /// </summary>
        /// <param name="when">A string that contains a date in the specified format.</param>
        /// <returns>A DateTime that was parsed.</returns>
        public static DateTime ParseDate(String when)
        {
            try
            {
                return DateTime.Parse(dateFormat);
            }
            catch (FormatException)
            {
                return default(DateTime);
            }
        }

        /// <summary>
        /// Construct an object to read the specified CSV file.
        /// </summary>
        /// <param name="filename">The filename to read.</param>
        public ReadCSV(String filename)
            : this(filename, true, ',')
        {
        }

        /// <summary>
        /// Construct the CSV reader.
        /// </summary>
        /// <param name="filename">The filename to read from.</param>
        /// <param name="headers">True if the first row specifies field names.</param>
        /// <param name="delim">The delimiter between each item.</param>
        public ReadCSV(String filename, bool headers, char delim)
        {
            this.reader = new StreamReader(filename);
            Begin(headers, delim);
        }


        /// <summary>
        /// Construct the CSV reader.
        /// </summary>
        /// <param name="istream">The input stream to read from.</param>
        /// <param name="headers">True if the first row specifies field names.</param>
        /// <param name="delim">The delimiter between the fields.</param>
        public ReadCSV(Stream istream, bool headers, char delim)
        {
            this.reader = new StreamReader(istream);
            Begin(headers, delim);
        }

        /// <summary>
        /// Begin the process of reading the CSV file.
        /// </summary>
        /// <param name="headers">The headers to read.</param>
        /// <param name="delim">The delimiter between the headers.</param>
        public void Begin(bool headers, char delim)
        {
            
            this.delim = delim;
            this.headers = headers;

            if (headers)
            {
                // read the column heads
                String line = this.reader.ReadLine();
                string[] tok = line.Split(delim);

                for (int index = 0; index < tok.Length; index++)
                {
                    String header = tok[index];
                    this.columns.Add(header.ToLower(), index);
                }
                this.data = new String[tok.Length];
            }


        }

        /// <summary>
        /// Close the file.
        /// </summary>
        public void Close()
        {
            this.reader.Close();
        }

        /// <summary>
        /// Get the specified column using an index.
        /// </summary>
        /// <param name="i">The zero based index of the column to read.</param>
        /// <returns>The specified column as a string.</returns>
        public String Get(int i)
        {
            return this.data[i];
        }

        /// <summary>
        /// Get the specified column as a string.
        /// </summary>
        /// <param name="column">The specified column.</param>
        /// <returns>The specified column as a string.</returns>
        public String Get(String column)
        {
            if (!columns.ContainsKey(column.ToLower()))
            {
                return null;
            }
            int i = this.columns[column.ToLower()];

            return this.data[i];
        }

        /// <summary>
        /// Read the specified column as a date.
        /// </summary>
        /// <param name="column">The specified column.</param>
        /// <returns>The specified column as a DateTime.</returns>
        public DateTime GetDate(String column)
        {
            String str = Get(column);
            return DateTime.Parse(str);
        }

        /// <summary>
        /// Get the specified column as a double.
        /// </summary>
        /// <param name="column">The column to read.</param>
        /// <returns>The specified column as a double.</returns>
        public double GetDouble(String column)
        {
            String str = Get(column);
            return double.Parse(str);
        }

        /// <summary>
        /// Get an integer that has the specified name.
        /// </summary>
        /// <param name="col">The column name to read.</param>
        /// <returns>The specified column as an int.</returns>
        public int GetInt(String col)
        {
            String str = Get(col);
            try
            {
                return int.Parse(str);
            }
            catch (FormatException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Read the next line.
        /// </summary>
        /// <returns>Return false if there are no more lines in the file.</returns>
        public bool Next()
        {
            String line = this.reader.ReadLine();
            if (line == null)
            {
                return false;
            }

            string[] tok = line.Split(',');

            if (this.data == null)
            {
                this.data = new String[tok.Length];
            }

            for (int i = 0; i < tok.Length; i++)
            {
                String str = tok[i];
                if (i < this.data.Length)
                {
                    this.data[i] = str;
                }
            }

            return true;
        }
    }
}
