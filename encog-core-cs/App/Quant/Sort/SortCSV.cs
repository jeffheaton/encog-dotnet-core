using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.Collections;
using System.IO;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Sort
{
    /// <summary>
    /// Used to sort a CSV file by one, or more, fields.
    /// </summary>
    public class SortCSV: BasicFile
    {
       /// <summary>
       /// Used to specify the sort order.
       /// </summary>
        public IList<SortedField> SortOrder
        {
            get
            {
                return this.sortOrder;
            }
        }

        /// <summary>
        /// The loaded rows.
        /// </summary>
        private List<LoadedRow> data = new List<LoadedRow>();

        /// <summary>
        /// The sort order.
        /// </summary>
        private IList<SortedField> sortOrder = new List<SortedField>();

        /// <summary>
        /// Read the input file.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="format">The format of the input file.</param>
        private void ReadInputFile()
        {
            ResetStatus();

            ReadCSV csv = new ReadCSV(InputFilename, ExpectInputHeaders, InputFormat);
            while (csv.Next())
            {
                UpdateStatus("Reading input file");
                LoadedRow row = new LoadedRow(csv);
                this.data.Add(row);
            }

            this.ColumnCount = csv.GetColumnCount();

            if (this.ExpectInputHeaders)
            {
                this.InputHeadings = csv.ColumnNames.ToArray<String>();
            }

            csv.Close();            
        }

        /// <summary>
        /// Sort the loaded data.
        /// </summary>
        private void SortData()
        {
            IComparer<LoadedRow> comp = new RowComparitor(this);
            this.data.Sort(comp);
        }

        /// <summary>
        /// Write the sorted output file. 
        /// </summary>
        /// <param name="outputFile">The name of the output file.</param>
        /// <param name="headers">True, if headers are to be written.</param>
        /// <param name="format">The format of the output file.</param>
        private void WriteOutputFile(String outputFile)
        {
            TextWriter tw = this.PrepareOutputFile(outputFile);

            bool[] nonNumeric = new bool[this.ColumnCount];
            bool first = true;

            ResetStatus();

            // write the file
            foreach (LoadedRow row in this.data)
            {
                UpdateStatus("Writing output");
                // for the first row, determine types
                if (first)
                {
                    for (int i = 0; i < this.ColumnCount; i++)
                    {
                        double d;
                        String str = row.Data[i];
                        nonNumeric[i] = !double.TryParse(str, out d);
                    }
                    first = false;
                }
                
                // write the row
                StringBuilder line = new StringBuilder();

                for (int i = 0; i < this.ColumnCount; i++)
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

        /// <summary>
        /// Process, and sort the files.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="headers">True, if headers are to be used.</param>
        /// <param name="format">The format of the file.</param>
        public void Process(String inputFile, String outputFile, bool headers, CSVFormat format)
        {
            this.InputFilename = inputFile;
            this.ExpectInputHeaders = headers;
            this.InputFormat = format;

            ReadInputFile();
            SortData();
            WriteOutputFile(outputFile);
        }
    }
}
