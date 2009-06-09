// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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
using log4net;
using Encog.Persist;

namespace Encog.Util
{
/**
 * Read and parse CSV format files.
 */
public class ReadCSV {

	        /// <summary>
        /// The format that dates are expected to be in.
        /// </summary>
        public const String dateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Format a date/time object to the same format that we parse in.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>A formatted date and time.</returns>
        public static String DisplayDate(DateTime date)
        {
            return date.ToString(dateFormat);
        }

	/**
	 * Get an array of double's from a string of comma separated text.
	 * 
	 * @param str
	 *            The string that contains a list of numbers.
	 * @return An array of doubles parsed from the string.
	 */
	public static double[] FromCommas( String str) {
		// first count the numbers
		
		 String[] tok = str.Split();
         int count = tok.Length;

		// now allocate an object to hold that many numbers
		 double[] result = new double[count];

		// and finally parse the numbers
		for( int index = 0;index<tok.Length;index++)
        {
			try {
                String num = tok[index];
				 double value = double.Parse(num);
				result[index] = value;
			} catch ( Exception e) {
				throw new PersistError(e);
			}

		}

		return result;
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

	/**
	 * Convert an array of doubles to a comma separated list.
	 * 
	 * @param result
	 *            This string will have the values appended to it.
	 * @param data
	 *            The array of doubles to use.
	 */
	public static void ToCommas( StringBuilder result, 
			 double[] data) {
		result.Length = 0;
		for (int i = 0; i < data.Length; i++) {
			if (i != 0) {
				result.Append(',');
			}
			result.Append(data[i]);
		}
	}

	        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ReadCSV));

        /// <summary>
        /// The file to read.
        /// </summary>
        private TextReader reader;

	/**
	 * The names of the columns.
	 */
	private  IDictionary<String, int> columns = new Dictionary<String, int>();

	/**
	 * The data.
	 */
	private String[] data;

	/**
	 * The delimiter.
	 */
	private  char delim;

	/**
	 * Construct a CSV reader from an input stream.
	 * 
	 * @param is
	 *            The InputStream to read from.
	 * @param headers
	 *            Are headers present?
	 * @param delim
	 *            What is the delimiter.
	 */
	public ReadCSV( Stream istream,  bool headers, 
				 char delim) {
                     this.reader = new StreamReader(istream);
		this.delim = delim;
		begin(headers);
	}

	/**
	 * Construct a CSV reader from a filename.
	 * 
	 * @param filename
	 *            The filename.
	 * @param headers
	 *            The headers.
	 * @param delim
	 *            The delimiter.
	 */
	public ReadCSV( String filename,  bool headers,
			 char delim) {
	        this.reader = new StreamReader(filename);
			this.delim = delim;
			begin(headers);

	}

	/**
	 * Reader the headers.
	 * 
	 * @param headers
	 *            Are headers present.
	 */
	private void begin( bool headers) {
		try {
			// read the column heads
			if (headers) {
				 String line = this.reader.ReadLine();
				 IList<String> tok = parse(line);

				int i = 0;
				foreach ( String header in tok) {
					this.columns.Add(header.ToLower(), i++);
				}
			}

			this.data = null;
		} catch ( IOException e) {
			if (this.logger.IsErrorEnabled) {
				this.logger.Error("Exception", e);
			}

			throw new EncogError(e);
		}
	}

	/**
	 * Close the file.
	 * 
	 */
	public void close() {
		try {
			this.reader.Close();
		} catch ( IOException e) {
			if (this.logger.IsErrorEnabled) {
				this.logger.Error("Exception", e);
			}

			throw new EncogError(e);
		}
	}

	/**
	 * Get the specified column as a string.
	 * 
	 * @param i
	 *            The column index, starting at zero.
	 * @return The column as a string.
	 */
	public String get( int i) {
		return this.data[i];
	}

	/**
	 * Get the column by its string name, as a string. This will only work if
	 * column headers were defined that have string names.
	 * 
	 * @param column
	 *            The column name.
	 * @return The column data as a string.
	 */
	public String Get( String column) {
        if(!this.columns.ContainsKey(column.ToLower()))
            return null;
		 int i = this.columns[column.ToLower()];
	
		return this.data[i];
	}

	/**
	 * Get the column count.
	 * 
	 * @return The column count.
	 */
	public int getColumnCount() {
		if (this.data == null) {
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
	/**
	 * Count the columns and create a an array to hold them.
	 * 
	 * @param line
	 *            One line from the file
	 */
	private void initData( String line) {
		 IList<String> tok = parse(line);
		this.data = new String[tok.Count];

	}

	/**
	 * Read the next line.
	 * 
	 * @return True if there are more lines to read.
	 */
	public bool next() {

		try {
			 String line = this.reader.ReadLine();
			if (line == null) {
				return false;
			}

			if (this.data == null) {
				initData(line);
			}

			 IList<String> tok = parse(line);

			int i = 0;
			foreach ( String str in tok) {
				if (i < this.data.Length) {
					this.data[i++] = str;
				}
			}

			return true;
		} catch ( IOException e) {
			if (this.logger.IsErrorEnabled) {
				this.logger.Error("Exception", e);
			}

			throw new EncogError(e);
		}

	}

	/**
	 * Parse the line into a list of values.
	 * @param line The line to parse.
	 * @return The elements on this line.
	 */
	private IList<String> parse( String line) {
		 StringBuilder item = new StringBuilder();
		 IList<String> result = new List<String>();
		bool quoted = false;

		for (int i = 0; i < line.Length; i++) {
			 char ch = line[i];
			if ((ch == this.delim) && !quoted) {
				result.Add(item.ToString());
				item.Length = 0;
				quoted = false;
			} else if ((ch == '\"') && (item.Length == 0)) {
				quoted = true;
			} else if ((ch == '\"') && quoted) {
				quoted = false;
			} else {
				item.Append(ch);
			}
		}

		if (item.Length > 0) {
			result.Add(item.ToString());
		}

		return result;
	}

}
}
