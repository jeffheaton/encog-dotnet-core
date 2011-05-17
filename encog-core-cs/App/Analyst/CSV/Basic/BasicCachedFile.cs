using System;
using System.Collections.Generic;
using System.IO;
using Encog.App.Quant;
using Encog.Util.CSV;
using Encog.Util.Logging;

namespace Encog.App.Analyst.CSV.Basic
{
    /// <summary>
    /// Forms the foundation of all of the cached files in Encog Quant.
    /// </summary>
    ///
    public class BasicCachedFile : BasicFile
    {
        /// <summary>
        /// The column mapping.
        /// </summary>
        ///
        private readonly IDictionary<String, BaseCachedColumn> columnMapping;

        /// <summary>
        /// The columns.
        /// </summary>
        ///
        private readonly IList<BaseCachedColumn> columns;

        public BasicCachedFile()
        {
            columnMapping = new Dictionary<String, BaseCachedColumn>();
            columns = new List<BaseCachedColumn>();
        }

        /// <value>The column mappings.</value>
        public IDictionary<String, BaseCachedColumn> ColumnMapping
        {
            /// <returns>The column mappings.</returns>
            get { return columnMapping; }
        }


        /// <value>The columns.</value>
        public IList<BaseCachedColumn> Columns
        {
            /// <returns>The columns.</returns>
            get { return columns; }
        }

        /// <summary>
        /// Add a new column.
        /// </summary>
        ///
        /// <param name="column">The column to add.</param>
        public void AddColumn(BaseCachedColumn column)
        {
            columns.Add(column);
            columnMapping[column.Name] = column;
        }

        /// <summary>
        /// Analyze the input file.
        /// </summary>
        ///
        /// <param name="input">The input file.</param>
        /// <param name="headers">True, if there are headers.</param>
        /// <param name="format">The format of the CSV data.</param>
        public virtual void Analyze(FileInfo input, bool headers,
                                    CSVFormat format)
        {
            ResetStatus();
            InputFilename = input;
            ExpectInputHeaders = headers;
            InputFormat = format;
            columnMapping.Clear();
            columns.Clear();

            // first count the rows
            TextReader reader = null;
            try
            {
                int recordCount = 0;
                reader = new StreamReader(InputFilename.OpenWrite());
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
                InputFormat = format;
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

                for (int i = 0; i < csv.ColumnNames.Count; i++)
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
                        InputFormat.Parse(str);
                        io = true;
                    }
                    catch (FormatException ex_0)
                    {
                        EncogLogging.Log(ex_0);
                    }

                    AddColumn(new FileData(name, i, io, io));
                }
            }
            finally
            {
                csv.Close();
                Analyzed = true;
            }
        }

        /// <summary>
        /// Attempt to resolve a column name.
        /// </summary>
        ///
        /// <param name="name">The unknown column name.</param>
        /// <returns>The known column name.</returns>
        private String AttemptResolveName(String name)
        {
            String name2 = name.ToLower();

            if (name2.IndexOf("open") != -1)
            {
                return FileData.OPEN;
            }
            else if (name2.IndexOf("close") != -1)
            {
                return FileData.CLOSE;
            }
            else if (name2.IndexOf("low") != -1)
            {
                return FileData.LOW;
            }
            else if (name2.IndexOf("hi") != -1)
            {
                return FileData.HIGH;
            }
            else if (name2.IndexOf("vol") != -1)
            {
                return FileData.VOLUME;
            }
            else if ((name2.IndexOf("date") != -1)
                     || (name.IndexOf("yyyy") != -1))
            {
                return FileData.DATE;
            }
            else if (name2.IndexOf("time") != -1)
            {
                return FileData.TIME;
            }

            return name;
        }

        /// <summary>
        /// Get the data for a specific column.
        /// </summary>
        ///
        /// <param name="name">The column to read.</param>
        /// <param name="csv">The CSV file to read from.</param>
        /// <returns>The column data.</returns>
        public String GetColumnData(String name, ReadCSV csv)
        {
            if (!columnMapping.ContainsKey(name))
            {
                return null;
            }

            BaseCachedColumn column = columnMapping[name];

            if (!(column is FileData))
            {
                return null;
            }

            var fd = (FileData) column;
            return csv.Get(fd.Index);
        }
    }
}