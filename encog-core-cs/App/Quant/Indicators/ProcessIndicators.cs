using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.IO;

namespace Encog.App.Quant.Indicators
{
    public class ProcessIndicators
    {
        public IDictionary<String, BaseColumn> ColumnMapping { get { return columnMapping; } }
        public IList<BaseColumn> Columns { get { return columns; } }
        public int Precision { get; set; }
        public bool Analyzed { get { return analyzed; } }

        private IDictionary<String, BaseColumn> columnMapping = new Dictionary<String, BaseColumn>();
        private IList<BaseColumn> columns = new List<BaseColumn>();
        private int recordCount;
        private String inputFilename;
        private bool inputHeaders;
        private CSVFormat inputFormat;
        private bool analyzed;

        public ProcessIndicators()
        {
            this.Precision = 10;
        }

        public int RecordCount
        {
            get
            {
                if (!Analyzed)
                {
                    throw new QuantError("Must analyze file first.");
                }
                return this.recordCount;
            }
        }

        public void Analyze(String input, bool headers, CSVFormat format)
        {
            this.columnMapping.Clear();
            this.columns.Clear();

            // first count the rows
            TextReader reader = null;
            try
            {                
                this.recordCount = 0;
                reader = new StreamReader(input);
                while (reader.ReadLine() != null)
                    this.recordCount++;

                if (headers)
                    this.recordCount--;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                this.inputFilename = input;
                this.inputHeaders = headers;
                this.inputFormat = format;
            }

            // now analyze columns
            ReadCSV csv = null;
            try
            {
                csv = new ReadCSV(input, headers, format);
                if (!csv.Next())
                {
                    throw new QuantError("File is empty");
                }

                for (int i = 0; i < csv.GetColumnCount(); i++)
                {
                    String name;

                    if (headers)
                        name = csv.ColumnNames[i];
                    else
                        name = "Column-" + (i + 1);

                    // determine if it should be an input/output field                    
                    double d;
                    String str = csv.Get(i);
                    bool io = double.TryParse(str,out d);

                    AddColumn(new FileData(name, i, io, io));
                }
            }
            finally
            {
                csv.Close();
                this.analyzed = true;
            }
        }

        public void AddColumn(BaseColumn column)
        {
            this.columns.Add(column);
            this.columnMapping[column.Name] = column;
        }

        private void ReadCSV()
        {
            ReadCSV csv = null;

            try
            {
                csv = new ReadCSV(inputFilename, this.inputHeaders, this.inputFormat);

                int row = 0;
                while (csv.Next())
                {
                    foreach( BaseColumn column in this.columns )
                    {
                        if (column is FileData)
                        {
                            if( column.Input )
                            {
                                FileData fd = (FileData)column;
                                String str = csv.Get(fd.Index);
                                double d = this.inputFormat.Parse(str);
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

            foreach (BaseColumn column in columns)
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
            int result = this.recordCount;

            foreach (BaseColumn column in columns)
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
                if (this.inputHeaders)
                {
                    StringBuilder line = new StringBuilder();

                    foreach (BaseColumn column in columns)
                    {
                        if (column.Output)
                        {
                            if (line.Length > 0)
                                line.Append(this.inputFormat.Separator);
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

                    foreach( BaseColumn column in columns)
                    {
                        if (column.Output)
                        {
                            if (line.Length > 0)
                                line.Append(this.inputFormat.Separator);
                            double d = column.Data[row];
                            line.Append(this.inputFormat.Format(d, Precision));
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
            foreach (BaseColumn column in this.columns)
            {
                column.Allocate(this.recordCount);
            }
        }

        private void CalculateIndicators()
        {
            foreach (BaseColumn column in this.columns)
            {
                if (column.Output)
                {
                    if (column is Indicator)
                    {
                        Indicator indicator = (Indicator)column;
                        indicator.Calculate(this.columnMapping, this.recordCount);
                    }
                }
            }
        }

        public void RenameColumn(int index, String newName)
        {
            this.columnMapping.Remove(columns[index].Name);
            this.columns[index].Name = newName;
            this.columnMapping.Add(newName, this.columns[index]);
        }

        public void Process(String output)
        {
            if (!analyzed)
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
