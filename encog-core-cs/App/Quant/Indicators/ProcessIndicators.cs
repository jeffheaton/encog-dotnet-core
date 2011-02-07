using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.IO;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Indicators
{
    /// <summary>
    /// Process indicators and generate output.
    /// </summary>
    public class ProcessIndicators : BasicCachedFile
    {        
        /// <summary>
        /// Read the CSV file.
        /// </summary>
        private void ReadCSV()
        {
            ReadCSV csv = null;

            try
            {
                csv = new ReadCSV(InputFilename, ExpectInputHeaders, InputFormat);

                ResetStatus();
                int row = 0;
                while (csv.Next())
                {
                    UpdateStatus("Reading data");
                    foreach (BaseCachedColumn column in this.Columns)
                    {
                        if (column is FileData)
                        {
                            if( column.Input )
                            {
                                FileData fd = (FileData)column;
                                String str = csv.Get(fd.Index);
                                double d = this.InputFormat.Parse(str);
                                fd.Data[row] = d;
                            }
                        }
                    }
                    row++;
                }
            }
            finally
            {
                ReportDone("Reading data");
                if (csv != null)
                    csv.Close();
            }
        }

        /// <summary>
        /// Get the beginning index.
        /// </summary>
        /// <returns></returns>
        private int GetBeginningIndex()
        {
            int result = 0;

            foreach (BaseCachedColumn column in Columns)
            {
                if (column is Indicator)
                {
                    Indicator ind = (Indicator)column;
                    result = Math.Max(ind.BeginningIndex, result);
                }
            }

            return result;
        }

        /// <summary>
        /// Get the ending index.
        /// </summary>
        /// <returns>The ending index.</returns>
        private int GetEndingIndex()
        {
            int result = this.RecordCount;

            foreach (BaseCachedColumn column in Columns)
            {
                if (column is Indicator)
                {
                    Indicator ind = (Indicator)column;
                    result = Math.Min(ind.EndingIndex, result);
                }
            }

            return result;
        }

        /// <summary>
        /// Write the CSV.
        /// </summary>
        /// <param name="filename">The target filename.</param>
        private void WriteCSV(String filename)
        {
            TextWriter tw = null;

            try
            {
                ResetStatus();
                tw = new StreamWriter(filename);

                // write the headers
                if (this.ExpectInputHeaders)
                {
                    StringBuilder line = new StringBuilder();

                    foreach (BaseCachedColumn column in Columns)
                    {
                        if (column.Output)
                        {
                            if (line.Length > 0)
                                line.Append(this.InputFormat.Separator);
                            line.Append("\"");
                            line.Append(column.Name);
                            line.Append("\"");
                        }
                    }

                    tw.WriteLine(line.ToString());
                }

                // starting and ending index
                int beginningIndex = GetBeginningIndex();
                int endingIndex = GetEndingIndex();

                // write the file data
                for (int row = beginningIndex; row <= endingIndex; row++)
                {
                    UpdateStatus("Writing data");
                    StringBuilder line = new StringBuilder();

                    foreach (BaseCachedColumn column in Columns)
                    {
                        if (column.Output)
                        {
                            if (line.Length > 0)
                                line.Append(this.InputFormat.Separator);
                            double d = column.Data[row];
                            line.Append(this.InputFormat.Format(d, Precision));
                        }
                    }

                    tw.WriteLine(line.ToString());
                }
            }
            finally
            {
                if (tw != null)
                    tw.Close();
            }
        }

        /// <summary>
        /// Allocate storage.
        /// </summary>
        private void AllocateStorage()
        {
            foreach (BaseCachedColumn column in this.Columns)
            {
                column.Allocate(this.RecordCount);
            }
        }

        /// <summary>
        /// Calculate the indicators.
        /// </summary>
        private void CalculateIndicators()
        {
            foreach (BaseCachedColumn column in this.Columns)
            {
                if (column.Output)
                {
                    if (column is Indicator)
                    {
                        Indicator indicator = (Indicator)column;
                        indicator.Calculate(this.ColumnMapping, this.RecordCount);
                    }
                }
            }
        }

        /// <summary>
        /// Rename a column.
        /// </summary>
        /// <param name="index">The column index.</param>
        /// <param name="newName">The new name.</param>
        public void RenameColumn(int index, String newName)
        {
            this.ColumnMapping.Remove(Columns[index].Name);
            this.Columns[index].Name = newName;
            this.ColumnMapping.Add(newName, this.Columns[index]);
        }

        /// <summary>
        /// Process and write the specified output file.
        /// </summary>
        /// <param name="output">The output file.</param>
        public void Process(String output)
        {
            ValidateAnalyzed();

            AllocateStorage();
            ReadCSV();
            CalculateIndicators();
            WriteCSV(output);
        }
    }
}
