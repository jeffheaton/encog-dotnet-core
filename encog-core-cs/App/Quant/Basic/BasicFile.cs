using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.IO;
using Encog.Engine;

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
        public IStatusReportable Report { get; set; }
        public int ReportInterval { get; set; }

        private int recordCount;
        private int lastUpdate;
        private int currentRecord;

        public BasicFile()
        {
            this.Report = new NullStatusReportable();
            this.ReportInterval = 10000;
            ResetStatus();
        }

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

        public void PerformBasicCounts()
        {
            ResetStatus();
            int recordCount = 0;
            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            while (csv.Next())
            {
                UpdateStatus(true);
                recordCount++;
            }
            this.RecordCount = recordCount;
            this.ColumnCount = csv.GetColumnCount();

            ReadHeaders(csv);
            csv.Close();
            ReportDone(true);
        }

        public void ReadHeaders(ReadCSV csv)
        {
            if (this.ExpectInputHeaders)
            {
                this.InputHeadings = new String[csv.ColumnNames.Count];
                for (int i = 0; i < csv.ColumnNames.Count; i++)
                {
                    this.InputHeadings[i] = csv.ColumnNames[i];
                }
            }
        }

        public void ResetStatus()
        {
            this.lastUpdate = 0;
            this.currentRecord = 0;
        }

        public void UpdateStatus(bool isAnalyzing)
        {
            bool shouldDisplay = false;

            if (this.currentRecord == 0)
            {
                shouldDisplay = true;
            }

            this.currentRecord++;
            this.lastUpdate++;

            if (lastUpdate > this.ReportInterval)
            {
                lastUpdate = 0;
                shouldDisplay = true;
            }


            if (shouldDisplay)
            {
                if (isAnalyzing)
                {
                    this.Report.Report(this.recordCount, this.currentRecord, "Analyzing");
                }
                else
                {
                    this.Report.Report(this.recordCount, this.currentRecord, "Processing");
                }
            }
        }

        public void ReportDone(bool isAnalyzing)
        {
                if (isAnalyzing)
                {
                    this.Report.Report(this.recordCount, this.recordCount, "Done analyzing");
                }
                else
                {
                    this.Report.Report(this.recordCount, this.recordCount, "Done processing");
                }
            }
    }
}
