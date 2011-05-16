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
        private readonly List<LoadedRow> data;

        /// <summary>
        /// The sort order.
        /// </summary>
        ///
        private readonly List<SortedField> sortOrder;

        public SortCSV()
        {
            data = new List<LoadedRow>();
            sortOrder = new List<SortedField>();
        }


        /// <value>Used to specify the sort order.</value>
        public IList<SortedField> SortOrder
        {
            /// <returns>Used to specify the sort order.</returns>
            get { return sortOrder; }
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
                data.Add(row);
            }

            Count = csv.ColumnNames.Count;

            if (ExpectInputHeaders)
            {
                InputHeadings = new String[csv.ColumnNames.Count];
                for (int i = 0; i < csv.ColumnNames.Count; i++)
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
            data.Sort(comp);
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
            foreach (LoadedRow row  in  data)
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
                        catch (Exception ex)
                        {
                            nonNumeric[i] = true;
                        }
                    }
                    first = false;
                }

                // write the row
                var line = new StringBuilder();

                for (int i_0 = 0; i_0 < Count; i_0++)
                {
                    if (i_0 > 0)
                    {
                        line.Append(",");
                    }

                    if (nonNumeric[i_0])
                    {
                        line.Append("\"");
                        line.Append(row.Data[i_0]);
                        line.Append("\"");
                    }
                    else
                    {
                        line.Append(row.Data[i_0]);
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