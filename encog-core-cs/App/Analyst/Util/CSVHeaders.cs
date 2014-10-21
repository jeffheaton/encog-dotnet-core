//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
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
using System.IO;
using System.Linq;
using System.Text;
using Encog.Util.CSV;

namespace Encog.App.Analyst.Util
{
    /// <summary>
    ///     Utility class to help deal with CSV headers.
    /// </summary>
    public class CSVHeaders
    {
        /// <summary>
        ///     The column mapping, maps column name to column index.
        /// </summary>
        private readonly IDictionary<String, Int32> _columnMapping;

        /// <summary>
        ///     The header list.
        /// </summary>
        private readonly IList<String> _headerList;

        /// <summary>
        ///     Construct the object.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="headers">False if headers are not extended.</param>
        /// <param name="format">The CSV format.</param>
        public CSVHeaders(FileInfo filename, bool headers,
                          CSVFormat format)
        {
            _headerList = new List<String>();
            _columnMapping = new Dictionary<String, Int32>();
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
                            _headerList.Add(str);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < csv.ColumnCount; i++)
                        {
                            _headerList.Add("field:" + (i + 1));
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
        ///     Construct the object.
        /// </summary>
        /// <param name="inputHeadings">The input headings.</param>
        public CSVHeaders(IEnumerable<string> inputHeadings)
        {
            _headerList = new List<String>();
            _columnMapping = new Dictionary<String, Int32>();

            foreach (String header  in  inputHeadings)
            {
                _headerList.Add(header);
            }
            Init();
        }

        /// <value>The headers.</value>
        public IList<String> Headers
        {
            get { return _headerList; }
        }

        /// <summary>
        ///     Parse a timeslice from a header such as (t-1).
        /// </summary>
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

            return (from v in values
                    select v.Trim()
                    into str where str.ToLower().StartsWith("t") select Int32.Parse(str.Substring(1))).FirstOrDefault();
        }

        /// <summary>
        ///     Tag a column with part # and timeslice.
        /// </summary>
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
        ///     Find the specified column.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <returns>The index of the column.</returns>
        public int Find(String name)
        {
            String key = name.ToLower();

            if (!_columnMapping.ContainsKey(key))
            {
                throw new AnalystError("Can't find column: " + name.ToLower());
            }

            return _columnMapping[key];
        }

        /// <summary>
        ///     Get the base header, strip any (...).
        /// </summary>
        /// <param name="index">The index of the header.</param>
        /// <returns>The base header.</returns>
        public String GetBaseHeader(int index)
        {
            String result = _headerList[index];

            int loc = result.IndexOf('(');
            if (loc != -1)
            {
                result = result.Substring(0, (loc) - (0));
            }

            return result.Trim();
        }

        /// <summary>
        ///     Get the specified header.
        /// </summary>
        /// <param name="index">The index of the header to get.</param>
        /// <returns>The header value.</returns>
        public String GetHeader(int index)
        {
            return _headerList[index];
        }


        /// <summary>
        ///     Get the timeslice for the specified index.
        /// </summary>
        /// <param name="currentIndex">The index to get the time slice for.</param>
        /// <returns>The timeslice.</returns>
        public int GetSlice(int currentIndex)
        {
            String name = _headerList[currentIndex];
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

            foreach (String v  in  values)
            {
                String str = v.Trim();
                if (str.ToLower().StartsWith("t"))
                {
                    str = v.Trim().Substring(1).Trim();
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
        ///     Setup the column mapping and validate.
        /// </summary>
        private void Init()
        {
            int index = 0;

            foreach (String str  in  _headerList)
            {
                _columnMapping[str.ToLower()] = index++;
            }

            ValidateSameName();
        }


        /// <returns>The number of headers.</returns>
        public int Size()
        {
            return _headerList.Count;
        }

        /// <summary>
        ///     Validate that two columns do not have the same name.  This is an error.
        /// </summary>
        private void ValidateSameName()
        {
            for (int i = 0; i < _headerList.Count; i++)
            {
                int i1 = i;
                int i2 = i;
                if (_headerList.Count > i2)
                    if (
                        _headerList.Where((t, j) => i1 != j)
                                   .Any(t => _headerList[i2].Equals(t, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        throw new AnalystError("Multiple fields named: "
                                               + _headerList[i]);
                    }
            }
        }
    }
}
