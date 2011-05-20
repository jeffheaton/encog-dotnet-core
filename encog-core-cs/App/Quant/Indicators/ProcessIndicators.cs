using System;
using System.IO;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.Util.CSV;

namespace Encog.App.Quant.Indicators
{
    /// <summary>
    /// Process indicators and generate output.
    /// </summary>
    ///
    public class ProcessIndicators : BasicCachedFile
    {
        /// <value>Get the beginning index.</value>
        private int BeginningIndex
        {
            get
            {
                int result = 0;


                foreach (BaseCachedColumn column  in  Columns)
                {
                    if (column is Indicator)
                    {
                        var ind = (Indicator) column;
                        result = Math.Max(ind.BeginningIndex, result);
                    }
                }

                return result;
            }
        }


        /// <value>Get the ending index.</value>
        private int EndingIndex
        {
            get
            {
                int result = RecordCount - 1;


                foreach (BaseCachedColumn column  in  Columns)
                {
                    if (column is Indicator)
                    {
                        var ind = (Indicator) column;
                        result = Math.Min(ind.EndingIndex, result);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Allocate storage.
        /// </summary>
        ///
        private void AllocateStorage()
        {
            foreach (BaseCachedColumn column  in  Columns)
            {
                column.Allocate(RecordCount);
            }
        }

        /// <summary>
        /// Calculate the indicators.
        /// </summary>
        ///
        private void CalculateIndicators()
        {
            foreach (BaseCachedColumn column  in  Columns)
            {
                if (column.Output)
                {
                    if (column is Indicator)
                    {
                        var indicator = (Indicator) column;
                        indicator.Calculate(ColumnMapping, RecordCount);
                    }
                }
            }
        }


        /// <summary>
        /// Process and write the specified output file.
        /// </summary>
        ///
        /// <param name="output">The output file.</param>
        public void Process(FileInfo output)
        {
            ValidateAnalyzed();

            AllocateStorage();
            ReadFile();
            CalculateIndicators();
            WriteCSV(output);
        }

        /// <summary>
        /// Read the CSV file.
        /// </summary>
        ///
        private void ReadFile()
        {
            ReadCSV csv = null;

            try
            {
                csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, InputFormat);

                ResetStatus();
                int row = 0;
                while (csv.Next() && !ShouldStop())
                {
                    UpdateStatus("Reading data");

                    foreach (BaseCachedColumn column  in  Columns)
                    {
                        if (column is FileData)
                        {
                            if (column.Input)
                            {
                                var fd = (FileData) column;
                                String str = csv.Get(fd.Index);
                                double d = InputFormat.Parse(str);
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
                {
                    csv.Close();
                }
            }
        }

        /// <summary>
        /// Rename a column.
        /// </summary>
        ///
        /// <param name="index">The column index.</param>
        /// <param name="newName">The new name.</param>
        public void RenameColumn(int index, String newName)
        {
            ColumnMapping.Remove(Columns[index].Name);
            Columns[index].Name = newName;
            ColumnMapping[newName] = Columns[index];
        }

        /// <summary>
        /// Write the CSV.
        /// </summary>
        ///
        /// <param name="filename">The target filename.</param>
        private void WriteCSV(FileInfo filename)
        {
            StreamWriter tw = null;

            try
            {
                ResetStatus();
                tw = new StreamWriter(filename.Create());

                // write the headers
                if (ExpectInputHeaders)
                {
                    var line = new StringBuilder();


                    foreach (BaseCachedColumn column  in  Columns)
                    {
                        if (column.Output)
                        {
                            if (line.Length > 0)
                            {
                                line.Append(InputFormat.Separator);
                            }
                            line.Append("\"");
                            line.Append(column.Name);
                            line.Append("\"");
                        }
                    }

                    tw.WriteLine(line.ToString());
                }

                // starting and ending index
                int beginningIndex = BeginningIndex;
                int endingIndex = EndingIndex;

                // write the file data
                for (int row = beginningIndex; row <= endingIndex; row++)
                {
                    UpdateStatus("Writing data");
                    var line_0 = new StringBuilder();


                    foreach (BaseCachedColumn column_1  in  Columns)
                    {
                        if (column_1.Output)
                        {
                            if (line_0.Length > 0)
                            {
                                line_0.Append(InputFormat.Separator);
                            }
                            double d = column_1.Data[row];
                            line_0.Append(InputFormat.Format(d, Precision));
                        }
                    }

                    tw.WriteLine(line_0.ToString());
                }
            }
            catch (IOException e)
            {
                throw (new QuantError(e));
            }
            finally
            {
                if (tw != null)
                {
                    tw.Close();
                }
            }
        }
    }
}