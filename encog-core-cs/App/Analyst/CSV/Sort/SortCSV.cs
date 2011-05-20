using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV.Sort
{
    /// <summary>
    /// Used to sort a CSV file by one, or more, fields.
    /// </summary>
    ///
    public class SortCSV : BasicFile
    {
        /// <summary>
        /// The loaded rows.
        /// </summary>
        ///
        private readonly List<LoadedRow> _data;

        /// <summary>
        /// The sort order.
        /// </summary>
        ///
        private readonly List<SortedField> _sortOrder;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public SortCSV()
        {
            _data = new List<LoadedRow>();
            _sortOrder = new List<SortedField>();
        }


        /// <value>Used to specify the sort order.</value>
        public IList<SortedField> SortOrder
        {
            get { return _sortOrder; }
        }


        /// <summary>
        /// Process, and sort the files.
        /// </summary>
        ///
        /// <param name="inputFile">The input file.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="headers">True, if headers are to be used.</param>
        /// <param name="format">The format of the file.</param>
        public void Process(FileInfo inputFile, FileInfo outputFile,
                            bool headers, CSVFormat format)
        {
            InputFilename = inputFile;
            ExpectInputHeaders = headers;
            InputFormat = format;

            ReadInputFile();
            SortData();
            WriteOutputFile(outputFile);
        }

        /// <summary>
        /// Read the input file.
        /// </summary>
        ///
        private void ReadInputFile()
        {
            ResetStatus();

            var csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, InputFormat);
            while (csv.Next() && !ShouldStop())
            {
                UpdateStatus("Reading input file");
                var row = new LoadedRow(csv);
                _data.Add(row);
            }

            Count = csv.ColumnCount;

            if (ExpectInputHeaders)
            {
                InputHeadings = new String[csv.ColumnCount];
                for (int i = 0; i < csv.ColumnCount; i++)
                {
                    InputHeadings[i] = csv.ColumnNames[i];
                }
            }

            csv.Close();
        }

        /// <summary>
        /// Sort the loaded data.
        /// </summary>
        ///
        private void SortData()
        {
            IComparer<LoadedRow> comp = new RowComparator(this);
            _data.Sort(comp);
        }

        /// <summary>
        /// Write the sorted output file.
        /// </summary>
        ///
        /// <param name="outputFile">The name of the output file.</param>
        private void WriteOutputFile(FileInfo outputFile)
        {
            StreamWriter tw = PrepareOutputFile(outputFile);
            var nonNumeric = new bool[Count];
            bool first = true;

            ResetStatus();


            // write the file
            foreach (LoadedRow row  in  _data)
            {
                UpdateStatus("Writing output");
                // for the first row, determine types
                if (first)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        try
                        {
                            String str = row.Data[i];
                            InputFormat.Parse(str);
                            nonNumeric[i] = false;
                        }
                        catch (Exception )
                        {
                            nonNumeric[i] = true;
                        }
                    }
                    first = false;
                }

                // write the row
                var line = new StringBuilder();

                for (int i = 0; i < Count; i++)
                {
                    if (i > 0)
                    {
                        line.Append(",");
                    }

                    if (nonNumeric[i])
                    {
                        line.Append("\"");
                        line.Append(row.Data[i]);
                        line.Append("\"");
                    }
                    else
                    {
                        line.Append(row.Data[i]);
                    }
                }

                tw.WriteLine(line.ToString());
            }

            ReportDone("Writing output");

            // close the file

            tw.Close();
        }
    }
}