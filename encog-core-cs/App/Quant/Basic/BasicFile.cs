using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.IO;
using Encog.Engine;

namespace Encog.App.Quant.Basic
{
    /// <summary>
    /// Many of the Encog quant CSV processors are based upon this class.  This class
    /// is not useful on its own. However, it does form the foundation for most Encog
    /// CSV file processing.
    /// </summary>
    public abstract class BasicFile
    {
        /// <summary>
        /// The column headings from the input file.
        /// </summary>
        public String[] InputHeadings { get; set; }

        /// <summary>
        /// The desired precision when numbers must be written.  Defaults to 10 decimal places.
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// Most Encog CSV classes must analyze a CSV file before actually processing it.  
        /// This property specifies if the file has been analyzed yet.
        /// </summary>
        public bool Analyzed { get; set; }

        /// <summary>
        /// The input filename.  This is the file being analyzed/processed.
        /// </summary>
        public String InputFilename { get; set; }

        /// <summary>
        /// True, if input headers should be expected.
        /// </summary>
        public bool ExpectInputHeaders { get; set; }

        /// <summary>
        /// The format of the input file.
        /// </summary>
        public CSVFormat InputFormat { get; set; }

        /// <summary>
        /// The number of columns in the input file.
        /// </summary>
        public int ColumnCount { get; set; }

        /// <summary>
        /// Allows status to be reported.  Defaults to no status reported.
        /// </summary>
        public IStatusReportable Report { get; set; }

        /// <summary>
        /// The number of records to process before status is updated.  Defaults to 10k.
        /// </summary>
        public int ReportInterval { get; set; }

        /// <summary>
        /// The number of records to process.  This is determined when the file is analyzed.
        /// </summary>
        private int recordCount;

        /// <summary>
        /// The last time status was updated.
        /// </summary>
        private int lastUpdate;

        /// <summary>
        /// The current record.
        /// </summary>
        private int currentRecord;


        /// <summary>
        /// Construct the object, and set the defaults.
        /// </summary>
        public BasicFile()
        {
            this.Report = new NullStatusReportable();
            this.ReportInterval = 10000;
            ResetStatus();
        }

        /// <summary>
        /// Prepare the output file, write headers if needed.
        /// </summary>
        /// <param name="outputFile">The name of the output file.</param>
        /// <returns>The output stream for the text file.</returns>
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

        /// <summary>
        /// Get the record count.  File must have been analyzed first to read the record count.
        /// </summary>
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

        /// <summary>
        /// Validate that the file has been analyzed.  Throw an error, if it has not.
        /// </summary>
        public void ValidateAnalyzed()
        {
            if (!Analyzed)
            {
                throw new QuantError("File must be analyzed first.");
            }
        }

        /// <summary>
        /// Write a row to the output file.
        /// </summary>
        /// <param name="tw">The output stream.</param>
        /// <param name="row">The row to write out.</param>
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

        /// <summary>
        /// Perform a basic analyze of the file.  This method is used mostly internally.
        /// </summary>
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

        /// <summary>
        /// Read the headers from a CSV file.  Used mostly internally.
        /// </summary>
        /// <param name="csv">The CSV file to read from.</param>
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

        /// <summary>
        /// Reset the reporting stats.  Used internally.
        /// </summary>
        public void ResetStatus()
        {
            this.lastUpdate = 0;
            this.currentRecord = 0;
        }

        /// <summary>
        /// Update the status.  Used internally.
        /// </summary>
        /// <param name="isAnalyzing">True if we are in the process of analyzing.</param>
        public void UpdateStatus(bool isAnalyzing)
        {
            if (isAnalyzing)
            {
                UpdateStatus("Analyzing");
            }
            else
            {
                UpdateStatus("Processing");
            }
        }

        /// <summary>
        /// Report that we are done.  Used internally.
        /// </summary>
        /// <param name="isAnalyzing">True if we are analyzing.</param>
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

        /// <summary>
        /// Report that we are done.  Used internally.
        /// </summary>
        /// <param name="task">The message.</param>
        public void ReportDone(String task)
        {
            this.Report.Report(this.recordCount, this.recordCount, task);
        }

        /// <summary>
        /// Report the current status.
        /// </summary>
        /// <param name="task">The string to report.</param>
        public void UpdateStatus(String task)
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
                this.Report.Report(this.recordCount, this.currentRecord, task);
            }
        }
    }
}
