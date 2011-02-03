using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Indicators;
using Encog.Util.CSV;
using System.IO;

namespace Encog.App.Quant.Basic
{
    public class BasicCachedFile: BasicFile
    {
        public IDictionary<String, BaseCachedColumn> ColumnMapping { get { return columnMapping; } }
        public IList<BaseCachedColumn> Columns { get { return columns; } }

        private IDictionary<String, BaseCachedColumn> columnMapping = new Dictionary<String, BaseCachedColumn>();
        private IList<BaseCachedColumn> columns = new List<BaseCachedColumn>();


        public virtual void Analyze(String input, bool headers, CSVFormat format)
        {
            this.ColumnMapping.Clear();
            this.Columns.Clear();

            // first count the rows
            TextReader reader = null;
            try
            {
                int recordCount = 0;
                reader = new StreamReader(input);
                while (reader.ReadLine() != null)
                    recordCount++;

                if (headers)
                    recordCount--;
                this.RecordCount = recordCount;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                this.InputFilename = input;
                this.ExpectInputHeaders = headers;
                this.InputFormat = format;
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
                        name = AttemptResolveName(csv.ColumnNames[i]);
                    else
                        name = "Column-" + (i + 1);

                    // determine if it should be an input/output field                    
                    double d;
                    String str = csv.Get(i);
                    bool io = double.TryParse(str, out d);

                    AddColumn(new FileData(name, i, io, io));
                }
            }
            finally
            {
                csv.Close();
                this.Analyzed = true;
            }
        }

        private String AttemptResolveName(String name)
        {
            name = name.ToLower();

            if (name.IndexOf("open") != -1)
            {
                return FileData.OPEN;
            }
            else if (name.IndexOf("close") != -1)
            {
                return FileData.CLOSE;
            }
            else if (name.IndexOf("low") != -1)
            {
                return FileData.LOW;
            }
            else if (name.IndexOf("hi") != -1)
            {
                return FileData.HIGH;
            }
            else if (name.IndexOf("vol") != -1)
            {
                return FileData.VOLUME;
            }
            else if (name.IndexOf("date") != -1 || name.IndexOf("yyyy")!=-1)
            {
                return FileData.DATE;
            }
            else if (name.IndexOf("time") != -1)
            {
                return FileData.TIME;
            }

            return name;
        }

        public void AddColumn(BaseCachedColumn column)
        {
            this.Columns.Add(column);
            this.ColumnMapping[column.Name] = column;
        }

        public String GetColumnData(String name, ReadCSV csv)
        {
            if (!this.ColumnMapping.ContainsKey(name))
                return null;

            BaseCachedColumn column = this.ColumnMapping[name];

            if (!(column is FileData))
                return null;

            FileData fd = (FileData)column;
            return csv.Get(fd.Index);
        }
    }
}
