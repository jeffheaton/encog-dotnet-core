using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Encog.Util.CSV;

namespace Encog.App.Analyst.Util
{
    /// <summary>
    /// Utility class to help deal with CSV headers.
    /// </summary>
    ///
    public class CSVHeaders
    {
        /// <summary>
        /// The column mapping, maps column name to column index.
        /// </summary>
        ///
        private readonly IDictionary<String, Int32> columnMapping;

        /// <summary>
        /// The header list.
        /// </summary>
        ///
        private readonly IList<String> headerList;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="filename">The filename.</param>
        /// <param name="headers">False if headers are not extended.</param>
        /// <param name="format">The CSV format.</param>
        public CSVHeaders(FileInfo filename, bool headers,
                          CSVFormat format)
        {
            headerList = new List<String>();
            columnMapping = new Dictionary<String, Int32>();
            ReadCSV csv = null;
            try
            {
                csv = new ReadCSV(filename.ToString(), headers, format);
                if (csv.Next())
                {
                    if (headers)
                    {
                        foreach (String str  in  csv.ColumnNames)
                        {
                            headerList.Add(str);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < csv.ColumnCount; i++)
                        {
                            headerList.Add("field:" + (i + 1));
                        }
                    }
                }

                Init();
            }
            finally
            {
                if (csv != null)
                {
                    csv.Close();
                }
            }
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="inputHeadings">The input headings.</param>
        public CSVHeaders(IList<String> inputHeadings)
        {
            headerList = new List<String>();
            columnMapping = new Dictionary<String, Int32>();

            foreach (String header  in  inputHeadings)
            {
                headerList.Add(header);
            }
            Init();
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="inputHeadings">The input headings.</param>
        public CSVHeaders(String[] inputHeadings)
        {
            headerList = new List<String>();
            columnMapping = new Dictionary<String, Int32>();

            foreach (String header  in  inputHeadings)
            {
                headerList.Add(header);
            }

            Init();
        }

        /// <value>The headers.</value>
        public IList<String> Headers
        {
            get { return headerList; }
        }

        /// <summary>
        /// Parse a timeslice from a header such as (t-1).
        /// </summary>
        ///
        /// <param name="name">The column name.</param>
        /// <returns>The timeslice.</returns>
        public static int ParseTimeSlice(String name)
        {
            int index1 = name.IndexOf('(');
            if (index1 == -1)
            {
                return 0;
            }
            int index2 = name.IndexOf(')');
            if (index2 == -1)
            {
                return 0;
            }
            if (index2 < index1)
            {
                return 0;
            }
            String list = name.Substring(index1 + 1, (index2) - (index1 + 1));
            String[] values = list.Split(',');

            foreach (String value_ren  in  values)
            {
                String str = value_ren.Trim();
                if (str.ToLower().StartsWith("t"))
                {
                    int slice = Int32.Parse(str.Substring(1));
                    return slice;
                }
            }

            return 0;
        }

        /// <summary>
        /// Tag a column with part # and timeslice.
        /// </summary>
        ///
        /// <param name="name">The name of the column.</param>
        /// <param name="part">The part #.</param>
        /// <param name="timeSlice">The timeslice.</param>
        /// <param name="multiPart">True if this is a multipart column.</param>
        /// <returns>The new tagged column.</returns>
        public static String TagColumn(String name, int part,
                                       int timeSlice, bool multiPart)
        {
            var result = new StringBuilder();
            result.Append(name);

            // is there any suffix?
            if (multiPart || (timeSlice != 0))
            {
                result.Append('(');

                // is there a part?
                if (multiPart)
                {
                    result.Append('p');
                    result.Append(part);
                }

                // is there a timeslice?
                if (timeSlice != 0)
                {
                    if (multiPart)
                    {
                        result.Append(',');
                    }
                    result.Append('t');
                    if (timeSlice > 0)
                    {
                        result.Append('+');
                    }
                    result.Append(timeSlice);
                }

                result.Append(')');
            }
            return result.ToString();
        }

        /// <summary>
        /// Find the specified column.
        /// </summary>
        ///
        /// <param name="name">The column name.</param>
        /// <returns>The index of the column.</returns>
        public int Find(String name)
        {
            String key = name.ToLower();

            if (!columnMapping.ContainsKey(key))
            {
                throw new AnalystError("Can't find column: " + name.ToLower());
            }

            return columnMapping[key];
        }

        /// <summary>
        /// Get the base header, strip any (...).
        /// </summary>
        ///
        /// <param name="index">The index of the header.</param>
        /// <returns>The base header.</returns>
        public String GetBaseHeader(int index)
        {
            String result = headerList[index];

            int loc = result.IndexOf('(');
            if (loc != -1)
            {
                result = result.Substring(0, (loc) - (0));
            }

            return result.Trim();
        }

        /// <summary>
        /// Get the specified header.
        /// </summary>
        ///
        /// <param name="index">The index of the header to get.</param>
        /// <returns>The header value.</returns>
        public String GetHeader(int index)
        {
            return headerList[index];
        }


        /// <summary>
        /// Get the timeslice for the specified index.
        /// </summary>
        ///
        /// <param name="currentIndex">The index to get the time slice for.</param>
        /// <returns>The timeslice.</returns>
        public int GetSlice(int currentIndex)
        {
            String name = headerList[currentIndex];
            int index1 = name.IndexOf('(');
            if (index1 == -1)
            {
                return 0;
            }
            int index2 = name.IndexOf(')');
            if (index2 == -1)
            {
                return 0;
            }
            if (index2 < index1)
            {
                return 0;
            }
            String list = name.Substring(index1 + 1, (index2) - (index1 + 1));
            String[] values = list.Split(',');

            foreach (String value_ren  in  values)
            {
                String str = value_ren.Trim();
                if (str.ToLower().StartsWith("t"))
                {
                    str = value_ren.Trim().Substring(1).Trim();
                    if (str[0] == '+')
                    {
                        // since Integer.parseInt can't handle +1
                        str = str.Substring(1);
                    }
                    int slice = Int32.Parse(str);
                    return slice;
                }
            }

            return 0;
        }

        /// <summary>
        /// Setup the column mapping and validate.
        /// </summary>
        ///
        private void Init()
        {
            int index = 0;

            foreach (String str  in  headerList)
            {
                columnMapping[str.ToLower()] = index++;
            }

            ValidateSameName();
        }


        /// <returns>The number of headers.</returns>
        public int Size()
        {
            return headerList.Count;
        }

        /// <summary>
        /// Validate that two columns do not have the same name.  This is an error.
        /// </summary>
        ///
        private void ValidateSameName()
        {
            for (int i = 0; i < headerList.Count; i++)
            {
                for (int j = 0; j < headerList.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (headerList[i].Equals(headerList[j], StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new AnalystError("Multiple fields named: "
                                               + headerList[i]);
                    }
                }
            }
        }
    }
}