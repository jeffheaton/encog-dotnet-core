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
using Encog.App.Quant;
using Encog.Util.CSV;
using Encog.Util.Logging;

namespace Encog.App.Analyst.CSV.Basic
{
    /// <summary>
    ///     Forms the foundation of all of the cached files in Encog Quant.
    /// </summary>
    public class BasicCachedFile : BasicFile
    {
        /// <summary>
        ///     The column mapping.
        /// </summary>
        private readonly IDictionary<String, BaseCachedColumn> _columnMapping;

        /// <summary>
        ///     The columns.
        /// </summary>
        private readonly IList<BaseCachedColumn> _columns;

        /// <summary>
        ///     Construct the object.
        /// </summary>
        public BasicCachedFile()
        {
            _columnMapping = new Dictionary<String, BaseCachedColumn>();
            _columns = new List<BaseCachedColumn>();
        }

        /// <value>The column mappings.</value>
        public IDictionary<String, BaseCachedColumn> ColumnMapping
        {
            get { return _columnMapping; }
        }


        /// <value>The columns.</value>
        public IList<BaseCachedColumn> Columns
        {
            get { return _columns; }
        }

        /// <summary>
        ///     Add a new column.
        /// </summary>
        /// <param name="column">The column to add.</param>
        public void AddColumn(BaseCachedColumn column)
        {
            _columns.Add(column);
            _columnMapping[column.Name] = column;
        }

        /// <summary>
        ///     Analyze the input file.
        /// </summary>
        /// <param name="input">The input file.</param>
        /// <param name="headers">True, if there are headers.</param>
        /// <param name="format">The format of the CSV data.</param>
        public virtual void Analyze(FileInfo input, bool headers,
                                    CSVFormat format)
        {
            ResetStatus();
            InputFilename = input;
            ExpectInputHeaders = headers;
            Format = format;
            _columnMapping.Clear();
            _columns.Clear();

            // first count the rows
            TextReader reader = null;
            try
            {
                int recordCount = 0;
                reader = new StreamReader(InputFilename.OpenRead());
                while (reader.ReadLine() != null)
                {
                    UpdateStatus(true);
                    recordCount++;
                }

                if (headers)
                {
                    recordCount--;
                }
                RecordCount = recordCount;
            }
            catch (IOException ex)
            {
                throw new QuantError(ex);
            }
            finally
            {
                ReportDone(true);
                if (reader != null)
                {
                    try
                    {
                        reader.Close();
                    }
                    catch (IOException e)
                    {
                        throw new QuantError(e);
                    }
                }
                InputFilename = input;
                ExpectInputHeaders = headers;
                Format = format;
            }

            // now analyze columns
            ReadCSV csv = null;
            try
            {
                csv = new ReadCSV(input.ToString(), headers, format);
                if (!csv.Next())
                {
                    throw new QuantError("File is empty");
                }

                for (int i = 0; i < csv.ColumnCount; i++)
                {
                    String name;

                    if (headers)
                    {
                        name = AttemptResolveName(csv.ColumnNames[i]);
                    }
                    else
                    {
                        name = "Column-" + (i + 1);
                    }

                    // determine if it should be an input/output field

                    String str = csv.Get(i);

                    bool io = false;

                    try
                    {
                        Format.Parse(str);
                        io = true;
                    }
                    catch (FormatException ex)
                    {
                        EncogLogging.Log(ex);
                    }

                    AddColumn(new FileData(name, i, io, io));
                }
            }
            finally
            {
                if (csv != null) csv.Close();
                Analyzed = true;
            }
        }

        /// <summary>
        ///     Attempt to resolve a column name.
        /// </summary>
        /// <param name="name">The unknown column name.</param>
        /// <returns>The known column name.</returns>
        private static String AttemptResolveName(String name)
        {
            String name2 = name.ToLower();

            if (name2.IndexOf("open") != -1)
            {
                return FileData.Open;
            }
            if (name2.IndexOf("close") != -1)
            {
                return FileData.Close;
            }
            if (name2.IndexOf("low") != -1)
            {
                return FileData.Low;
            }
            if (name2.IndexOf("hi") != -1)
            {
                return FileData.High;
            }
            if (name2.IndexOf("vol") != -1)
            {
                return FileData.Volume;
            }
            if ((name2.IndexOf("date") != -1)
                || (name.IndexOf("yyyy") != -1))
            {
                return FileData.Date;
            }
            if (name2.IndexOf("time") != -1)
            {
                return FileData.Time;
            }

            return name;
        }

        /// <summary>
        ///     Get the data for a specific column.
        /// </summary>
        /// <param name="name">The column to read.</param>
        /// <param name="csv">The CSV file to read from.</param>
        /// <returns>The column data.</returns>
        public String GetColumnData(String name, ReadCSV csv)
        {
            if (!_columnMapping.ContainsKey(name))
            {
                return null;
            }

            BaseCachedColumn column = _columnMapping[name];

            if (!(column is FileData))
            {
                return null;
            }

            var fd = (FileData) column;
            return csv.Get(fd.Index);
        }
    }
}
