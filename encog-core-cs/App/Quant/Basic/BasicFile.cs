using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.IO;

namespace Encog.App.Quant.Basic
{
    public class BasicFile
    {
        public CSVFormat Format { get; set; }
        public String[] InputHeadings { get; set; }

        public int Precision { get; set; }
        public bool Analyzed { get; set; }
        public String InputFilename { get; set; }
        public bool ExpectInputHeaders { get; set; }
        public CSVFormat InputFormat { get; set; }
        public int ColumnCount { get; set; }

        private int recordCount;

        public TextWriter PrepareOutputFile(String outputFile)
        {
            TextWriter tw = new StreamWriter(outputFile);

            // write headers, if needed
            if (ExpectInputHeaders)
            {
                int index = 0;
                StringBuilder line = new StringBuilder();
                foreach (String str in this.InputHeadings)
                {
                    if (line.Length > 0)
                    {
                        line.Append(",");
                    }
                    line.Append("\"");
                    line.Append(this.InputHeadings[index++]);
                    line.Append("\"");
                }
                tw.WriteLine(line.ToString());
            }

            return tw;
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
            set
            {
                this.recordCount = value;
            }
        }

        public void ValidateAnalyzed()
        {
            if (!Analyzed)
            {
                throw new QuantError("File must be analyzed first.");
            }
        }

        public void WriteRow(TextWriter tw, LoadedRow row)
        {
            StringBuilder line = new StringBuilder();

            for (int i = 0; i < this.ColumnCount; i++)
            {
                if (i > 0)
                {
                    line.Append(",");
                }

                /*if (nonNumeric[i])
                {
                    line.Append("\"");
                    line.Append(row.Data[i]);
                    line.Append("\"");
                }
                else*/
                {
                    line.Append(row.Data[i]);
                }
            }

            tw.WriteLine(line.ToString());
        }

    }
}
