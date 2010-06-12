// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Encog.Persist;
using System.Globalization;

#if logging
using log4net;
#endif
namespace Encog.Util.CSV
{
    /// <summary>
    /// Read and parse CSV format files.
    /// </summary>
    public class ReadCSV
    {

        /// <summary>
        /// The format that dates are expected to be in.
        /// </summary>
        public const String dateFormat = "yyyy-MM-dd";

        private CSVFormat format;

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
        /// Parse a date using the specified format.
        /// </summary>
        /// <param name="when">A string that contains a date in the specified format.</param>
        /// <returns>A DateTime that was parsed.</returns>
        public static DateTime ParseDate(String when)
        {
            try
            {
                return DateTime.ParseExact(when, dateFormat, 
                    CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                return default(DateTime);
            }
        }


#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ReadCSV));
#endif
        /// <summary>
        /// The file to read.
        /// </summary>
        private TextReader reader;

        /// <summary>
        /// The names of the columns.
        /// </summary>
        private IDictionary<String, int> columns = new Dictionary<String, int>();

        /// <summary>
        /// The data.
        /// </summary>
        private String[] data;

        /// <summary>
        /// The delimiter.
        /// </summary>
        private char delim;

        /// <summary>
        /// Construct a CSV reader from an input stream.
        /// </summary>
        /// <param name="istream">The InputStream to read from.</param>
        /// <param name="headers">Are headers present?</param>
        /// <param name="delim">What is the delimiter.</param>
        public ReadCSV(Stream istream, bool headers,
                     char delim)
        {
            CSVFormat format = new CSVFormat(CSVFormat.DecimalCharacter, delim);
            this.reader = new StreamReader(istream);
            this.delim = delim;
            Begin(headers, format);
        }

        /// <summary>
        /// Construct a CSV reader from a filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="delim">The delimiter.</param>
        public ReadCSV(String filename, bool headers,
                 char delim)
        {
            CSVFormat format = new CSVFormat(CSVFormat.DecimalCharacter, delim);
            this.reader = new StreamReader(filename);
            this.delim = delim;
            Begin(headers, format);

        }

        /// <summary>
        /// Construct a CSV reader from a filename.
        /// Allows a delimiter character to be specified.
        /// Numbers will be parsed using the current
        /// locale.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="format">The delimiter.</param>
        public ReadCSV(String filename, bool headers,
                 CSVFormat format)
        {

            this.reader = new StreamReader(filename);
            Begin(headers, format);

        }
        
        /// <summary>
        /// Construct a CSV reader from an input stream.
        /// The format parameter specifies the separator 
        /// character to use, as well as the number
        /// format.
        /// </summary>
        /// <param name="stream">The Stream to read from.</param>
        /// <param name="headers">Are headers present?</param>
        /// <param name="format">What is the CSV format.</param>
        public ReadCSV(Stream stream, bool headers,
                     CSVFormat format)
        {
            this.reader = new StreamReader(stream);
            Begin(headers, format);
        }


        /// <summary>
        /// Read the headers.
        /// </summary>
        /// <param name="format">The format of this CSV file.</param>
        /// <param name="headers">Are headers present.</param>
        private void Begin(bool headers, CSVFormat format)
        {
            try
            {
                this.format = format;
                // read the column heads
                if (headers)
                {
                    String line = this.reader.ReadLine();
                    IList<String> tok = Parse(line);

                    int i = 0;
                    foreach (String header in tok)
                    {
                        this.columns.Add(header.ToLower(), i++);
                    }
                }

                this.data = null;
            }
            catch (IOException e)
            {
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error("Exception", e);
                }
#endif
                throw new EncogError(e);
            }
        }

        /// <summary>
        /// Close the file.
        /// </summary>
        public void Close()
        {
            try
            {
                this.reader.Close();
            }
            catch (IOException e)
            {
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error("Exception", e);
                }
#endif
                throw new EncogError(e);
            }
        }

        /// <summary>
        /// Get the specified column as a string.
        /// </summary>
        /// <param name="i">The column index, starting at zero.</param>
        /// <returns>The column as a string.</returns>
        public String Get(int i)
        {
            return this.data[i];
        }

        /// <summary>
        /// Get the column by its string name, as a string. This will only work if
        /// column headers were defined that have string names.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>The column data as a string.</returns>
        public String Get(String column)
        {
            if (!this.columns.ContainsKey(column.ToLower()))
                return null;
            int i = this.columns[column.ToLower()];

            return this.data[i];
        }

        /// <summary>
        /// Get the column count.
        /// </summary>
        /// <returns>The column count.</returns>
        public int GetColumnCount()
        {
            if (this.data == null)
            {
                return 0;
            }

            return this.data.Length;
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
            return this.format.Parse(str);
        }

        /// <summary>
        /// Get the specified column as a double.
        /// </summary>
        /// <param name="column">The column to read.</param>
        /// <returns>The specified column as a double.</returns>
        public double GetDouble(int column)
        {
            String str = Get(column);
            return this.format.Parse(str);
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
        /// Count the columns and create a an array to hold them.
        /// </summary>
        /// <param name="line">One line from the file</param>
        private void InitData(String line)
        {
            IList<String> tok = Parse(line);
            this.data = new String[tok.Count];

        }


        /// <summary>
        /// Read the next line.
        /// </summary>
        /// <returns>True if there are more lines to read.</returns>
        public bool Next()
        {

            try
            {
                String line = this.reader.ReadLine();
                if (line == null)
                {
                    return false;
                }

                if (this.data == null)
                {
                    InitData(line);
                }

                IList<String> tok = Parse(line);

                int i = 0;
                foreach (String str in tok)
                {
                    if (i < this.data.Length)
                    {
                        this.data[i++] = str;
                    }
                }

                return true;
            }
            catch (IOException e)
            {
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error("Exception", e);
                }
#endif
                throw new EncogError(e);
            }

        }


        /// <summary>
        /// Parse the line into a list of values.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The elements on this line.</returns>
        private IList<String> Parse(String line)
        {
            StringBuilder item = new StringBuilder();
            IList<String> result = new List<String>();
            bool quoted = false;

            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                if ((ch == this.format.Separator) && !quoted)
                {
                    result.Add(item.ToString());
                    item.Length = 0;
                    quoted = false;
                }
                else if ((ch == '\"') && (item.Length == 0))
                {
                    quoted = true;
                }
                else if ((ch == '\"') && quoted)
                {
                    quoted = false;
                }
                else
                {
                    item.Append(ch);
                }
            }

            if (item.Length > 0)
            {
                result.Add(item.ToString());
            }

            return result;
        }

    }
}
