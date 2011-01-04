using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.IO;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Indicators
{
    public class ProcessIndicators : BasicFinancialFile
    {        
        public ProcessIndicators()
        {
            this.Precision = 10;
        }

        private void ReadCSV()
        {
            ReadCSV csv = null;

            try
            {
                csv = new ReadCSV(InputFilename, InputHeaders, InputFormat);

                int row = 0;
                while (csv.Next())
                {
                    foreach( BaseColumn column in this.Columns )
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
                if (csv != null)
                    csv.Close();
            }
        }

        private int GetBeginningIndex()
        {
            int result = 0;

            foreach (BaseColumn column in Columns)
            {
                if (column is Indicator)
                {
                    Indicator ind = (Indicator)column;
                    result = Math.Max(ind.BeginningIndex, result);
                }
            }

            return result;
        }

        private int GetEndingIndex()
        {
            int result = this.RecordCount;

            foreach (BaseColumn column in Columns)
            {
                if (column is Indicator)
                {
                    Indicator ind = (Indicator)column;
                    result = Math.Min(ind.EndingIndex, result);
                }
            }

            return result;
        }

        private void WriteCSV(String filename)
        {
            TextWriter tw = null;

            try
            {
                tw = new StreamWriter(filename);

                // write the headers
                if (this.InputHeaders)
                {
                    StringBuilder line = new StringBuilder();

                    foreach (BaseColumn column in Columns)
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
                    StringBuilder line = new StringBuilder();

                    foreach( BaseColumn column in Columns)
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

        private void AllocateStorage()
        {
            foreach (BaseColumn column in this.Columns)
            {
                column.Allocate(this.RecordCount);
            }
        }

        private void CalculateIndicators()
        {
            foreach (BaseColumn column in this.Columns)
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

        public void RenameColumn(int index, String newName)
        {
            this.ColumnMapping.Remove(Columns[index].Name);
            this.Columns[index].Name = newName;
            this.ColumnMapping.Add(newName, this.Columns[index]);
        }

        public void Process(String output)
        {
            if (!Analyzed)
            {
                throw new QuantError("File must be analyzed first.");
            }

            AllocateStorage();
            ReadCSV();
            CalculateIndicators();
            WriteCSV(output);
        }

        public bool Calculate(double[] inputData, double[] outputData)
        {
            return false;
        }
    }
}
