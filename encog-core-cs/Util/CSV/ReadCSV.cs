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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

#if logging

#endif

namespace Encog.Util.CSV
{
    /// <summary>
    /// Read and parse CSV format files.
    /// </summary>
    public class ReadCSV
    {
        /// <summary>
        /// The names of the columns.
        /// </summary>
        private readonly IList<String> _columnNames = new List<String>();

        /// <summary>
        /// The names of the columns.
        /// </summary>
        private readonly IDictionary<String, int> _columns = new Dictionary<String, int>();

        /// <summary>
        /// The file to read.
        /// </summary>
        private readonly TextReader _reader;

        /// <summary>
        /// The data.
        /// </summary>
        private String[] _data;

        /// <summary>
        /// The delimiter.
        /// </summary>
        private char _delim;

        private CSVFormat _format;

        /// <summary>
        /// Construct a CSV reader from an input stream.
        /// </summary>
        /// <param name="istream">The InputStream to read from.</param>
        /// <param name="headers">Are headers present?</param>
        /// <param name="delim">What is the delimiter.</param>
        public ReadCSV(Stream istream, bool headers,
                       char delim)
        {
            var format = new CSVFormat(CSVFormat.DecimalCharacter, delim);
            _reader = new StreamReader(istream);
            _delim = delim;
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
            var format = new CSVFormat(CSVFormat.DecimalCharacter, delim);
            _reader = new StreamReader(filename);
            _delim = delim;
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
            _reader = new StreamReader(filename);
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
            _reader = new StreamReader(stream);
            Begin(headers, format);
        }

        /// <summary>
        /// The format that dates are expected to be in. (i.e. "yyyy-MM-dd")
        /// </summary>
        public String DateFormat { get; set; }

        /// <summary>
        /// The format that times are expected to be in. (i.e. "hhmmss").
        /// </summary>
        public String TimeFormat { get; set; }

        /// <summary>
        /// The current format.
        /// </summary>
        public CSVFormat Format
        {
            get { return _format; }
        }

        /// <summary>
        /// The names of the columns.
        /// </summary>
        public IList<String> ColumnNames
        {
            get { return _columnNames; }
        }

        /// <summary>
        /// Return the number of columns, if known. 
        /// </summary>
        public int ColumnCount
        {
            get
            {
                if (_data == null)
                {
                    return 0;
                }
                return _data.Length;
            }
        }

        /// <summary>
        /// Parse a date using the specified format.
        /// </summary>
        /// <param name="when">A string that contains a date in the specified format.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <returns>A DateTime that was parsed.</returns>
        public static DateTime ParseDate(String when, String dateFormat)
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


        /// <summary>
        /// Read the headers.
        /// </summary>
        /// <param name="format">The format of this CSV file.</param>
        /// <param name="headers">Are headers present.</param>
        private void Begin(bool headers, CSVFormat format)
        {
            try
            {
                DateFormat = "yyyy-MM-dd";
                TimeFormat = "hhmmss";
                _format = format;
                // read the column heads
                if (headers)
                {
                    String line = _reader.ReadLine();
                    IList<String> tok = Parse(line);

                    int i = 0;
                    foreach (String header in tok)
                    {
                        if (_columns.ContainsKey(header.ToLower()))
                            throw new EncogError("Two columns cannot have the same name");
                        _columns.Add(header.ToLower(), i++);
                        _columnNames.Add(header);
                    }
                }

                _data = null;
            }
            catch (IOException e)
            {
#if logging
                if (logger.IsErrorEnabled)
                {
                    logger.Error("Exception", e);
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
                _reader.Close();
            }
            catch (IOException e)
            {
#if logging
                if (logger.IsErrorEnabled)
                {
                    logger.Error("Exception", e);
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
            return _data[i];
        }

        /// <summary>
        /// Get the column by its string name, as a string. This will only work if
        /// column headers were defined that have string names.
        /// </summary>
        /// <param name="column">The column name.</param>
        /// <returns>The column data as a string.</returns>
        public String Get(String column)
        {
            if (!_columns.ContainsKey(column.ToLower()))
                return null;
            int i = _columns[column.ToLower()];

            return _data[i];
        }

        /// <summary>
        /// Get the column count.
        /// </summary>
        /// <returns>The column count.</returns>
        public int GetCount()
        {
            if (_data == null)
            {
                return 0;
            }

            return _data.Length;
        }

        /// <summary>
        /// Read the specified column as a date.
        /// </summary>
        /// <param name="column">The specified column.</param>
        /// <returns>The specified column as a DateTime.</returns>
        public DateTime GetDate(String column)
        {
            String str = Get(column);
            return DateTime.ParseExact(str, DateFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Read the specified column as a time.
        /// </summary>
        /// <param name="column">The specified column.</param>
        /// <returns>The specified column as a DateTime.</returns>
        public DateTime GetTime(String column)
        {
            String str = Get(column);
            return DateTime.ParseExact(str, TimeFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Read the specified column as a date.
        /// </summary>
        /// <param name="column">The specified column.</param>
        /// <returns>The specified column as a DateTime.</returns>
        public DateTime GetDate(int column)
        {
            String str = Get(column);
            return DateTime.ParseExact(str, DateFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Read the specified column as a time.
        /// </summary>
        /// <param name="column">The specified column.</param>
        /// <returns>The specified column as a DateTime.</returns>
        public DateTime GetTime(int column)
        {
            String str = Get(column);
            return DateTime.ParseExact(str, TimeFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get the specified column as a double.
        /// </summary>
        /// <param name="column">The column to read.</param>
        /// <returns>The specified column as a double.</returns>
        public double GetDouble(String column)
        {
            String str = Get(column);
            return _format.Parse(str);
        }

        /// <summary>
        /// Get the specified column as a double.
        /// </summary>
        /// <param name="column">The column to read.</param>
        /// <returns>The specified column as a double.</returns>
        public double GetDouble(int column)
        {
            String str = Get(column);
            return _format.Parse(str);
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
        private void InitData(string line)
        {
            IList<String> tok = Parse(line);
            _data = new String[tok.Count];
        }


        /// <summary>
        /// Read the next line.
        /// </summary>
        /// <returns>True if there are more lines to read.</returns>
        public bool Next()
        {
            try
            {
                String line;

                do
                {
                    line = _reader.ReadLine();
                } while ((line != null) && line.Trim().Length == 0);

                if (line == null)
                {
                    return false;
                }

                if (_data == null)
                {
                    InitData(line);
                }

                IList<String> tok = Parse(line);

                int i = 0;
                foreach (String str in tok)
                {
                    if (i < _data.Length)
                    {
                        _data[i++] = str;
                    }
                }

                return true;
            }
            catch (IOException e)
            {
#if logging
                if (logger.IsErrorEnabled)
                {
                    logger.Error("Exception", e);
                }
#endif
                throw new EncogError(e);
            }
        }

        private IList<String> Parse(string line)
        {
            if (Format.Separator == ' ')
            {
                return ParseSpaceSep(line);
            }
            else
            {
                return ParseCharSep(line);
            }
        }

        /// <summary>
        /// Parse the line into a list of values.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The elements on this line.</returns>
        private IList<String> ParseCharSep(string line)
        {
            var item = new StringBuilder();
            var result = new List<String>();
            bool quoted = false;
            bool hadQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                if ((ch == Format.Separator) && !quoted)
                {
                    String s = item.ToString();
                    if (!hadQuotes)
                    {
                        s = s.Trim();
                    }
                    result.Add(s);
                    item.Length = 0;
                    quoted = false;
                    hadQuotes = false;
                }
                else if ((ch == '\"') && quoted)
                {
                    quoted = false;
                }
                else if ((ch == '\"') && (item.Length == 0))
                {
                    hadQuotes = true;
                    quoted = true;
                }
                else
                {
                    item.Append(ch);
                }
            }

            if (item.Length > 0)
            {
                String s = item.ToString();
                if (!hadQuotes)
                {
                    s = s.Trim();
                }
                result.Add(s);
            }

            return result;
        }

        /// <summary>
        /// Parse a line with space separators.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The list of items from the line.</returns>
        private static List<String> ParseSpaceSep(String line)
        {
            var result = new List<String>();
            var parse = new SimpleParser(line);

            while (!parse.EOL())
            {
                result.Add(parse.Peek() == '\"' ? parse.ReadQuotedString() : parse.ReadToWhiteSpace());
                parse.EatWhiteSpace();
            }

            return result;
        }


        internal long GetLong(int col)
        {
            String str = Get(col);
            try
            {
                return long.Parse(str);
            }
            catch (FormatException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Check to see if there are any missing values on the current row.
        /// </summary>
        /// <returns>True, if there are missing values.</returns>
        public bool HasMissing()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                String s = _data[i].Trim();
                if (s.Length == 0 || s.Equals("?"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}