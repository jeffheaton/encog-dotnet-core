using System;
using System.IO;
using System.Text;
using Encog.App.Analyst.Script;
using Encog.App.Quant;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV.Basic
{
    /// <summary>
    /// Many of the Encog quant CSV processors are based upon this class. This class
    /// is not useful on its own. However, it does form the foundation for most Encog
    /// CSV file processing.
    /// </summary>
    ///
    public class BasicFile : QuantTask
    {
        /// <summary>
        /// The default report interval.
        /// </summary>
        ///
        public const int REPORT_INTERVAL = 10000;

        /// <summary>
        /// Most Encog CSV classes must analyze a CSV file before actually processing
        /// it. This property specifies if the file has been analyzed yet.
        /// </summary>
        ///
        private bool analyzed;

        /// <summary>
        /// True, if the process should stop.
        /// </summary>
        ///
        private bool cancel;

        /// <summary>
        /// The number of columns in the input file.
        /// </summary>
        ///
        private int columnCount;

        /// <summary>
        /// The current record.
        /// </summary>
        ///
        private int currentRecord;

        /// <summary>
        /// True, if input headers should be expected.
        /// </summary>
        ///
        private bool expectInputHeaders;

        /// <summary>
        /// The input filename. This is the file being analyzed/processed.
        /// </summary>
        ///
        private FileInfo inputFilename;

        /// <summary>
        /// The format of the input file.
        /// </summary>
        ///
        private CSVFormat inputFormat;

        /// <summary>
        /// The column headings from the input file.
        /// </summary>
        ///
        private String[] inputHeadings;

        /// <summary>
        /// The last time status was updated.
        /// </summary>
        ///
        private int lastUpdate;

        /// <summary>
        /// The output format, usually, the same as the input format.
        /// </summary>
        ///
        private CSVFormat outputFormat;

        /// <summary>
        /// Should output headers be produced?
        /// </summary>
        ///
        private bool produceOutputHeaders;

        /// <summary>
        /// The number of records to process. This is determined when the file is
        /// analyzed.
        /// </summary>
        ///
        private int recordCount;

        /// <summary>
        /// Allows status to be reported. Defaults to no status reported.
        /// </summary>
        ///
        private IStatusReportable report;

        /// <summary>
        /// The number of records to process before status is updated. Defaults to
        /// 10k.
        /// </summary>
        ///
        private int reportInterval;

        /// <summary>
        /// Construct the object, and set the defaults.
        /// </summary>
        ///
        public BasicFile()
        {
            Precision = EncogFramework.DEFAULT_PRECISION;
            report = new NullStatusReportable();
            reportInterval = REPORT_INTERVAL;
            produceOutputHeaders = true;
            ResetStatus();
        }

        /// <summary>
        /// Set the column count.
        /// </summary>
        ///
        /// <value>The new column count.</value>
        public int Count
        {
            /// <returns>The column count.</returns>
            get { return columnCount; }
            /// <summary>
            /// Set the column count.
            /// </summary>
            ///
            /// <param name="theCount">The new column count.</param>
            set { columnCount = value; }
        }


        /// <summary>
        /// Set the input filename.
        /// </summary>
        ///
        /// <value>The input filename.</value>
        public FileInfo InputFilename
        {
            /// <returns>The input filename.</returns>
            get { return inputFilename; }
            /// <summary>
            /// Set the input filename.
            /// </summary>
            ///
            /// <param name="theInputFilename">The input filename.</param>
            set { inputFilename = value; }
        }


        /// <summary>
        /// Set the input format.
        /// </summary>
        ///
        /// <value>The new inputFormat format.</value>
        public CSVFormat InputFormat
        {
            /// <returns>THe input format.</returns>
            get { return inputFormat; }
            /// <summary>
            /// Set the input format.
            /// </summary>
            ///
            /// <param name="theInputFormat">The new inputFormat format.</param>
            set { inputFormat = value; }
        }


        /// <summary>
        /// Set the input headings.
        /// </summary>
        ///
        /// <value>The new input headings.</value>
        public String[] InputHeadings
        {
            /// <returns>The input headings.</returns>
            get { return inputHeadings; }
            /// <summary>
            /// Set the input headings.
            /// </summary>
            ///
            /// <param name="theInputHeadings">The new input headings.</param>
            set { inputHeadings = value; }
        }


        /// <value>the outputFormat to set</value>
        public CSVFormat OutputFormat
        {
            /// <returns>the outputFormat</returns>
            get { return outputFormat; }
            /// <param name="theOutputFormat">the outputFormat to set</param>
            set { outputFormat = value; }
        }


        /// <summary>
        /// Set the precision to use.
        /// </summary>
        ///
        /// <value>The precision to use.</value>
        public int Precision { /// <returns>The precision to use.</returns>
            get; /// <summary>
            /// Set the precision to use.
            /// </summary>
            ///
            /// <param name="thePrecision">The precision to use.</param>
            set; }


        /// <summary>
        /// Set the record count.
        /// </summary>
        ///
        /// <value>The record count.</value>
        public int RecordCount
        {
            /// <returns>Get the record count. File must have been analyzed first to read
            /// the record count.</returns>
            get
            {
                if (!analyzed)
                {
                    throw new QuantError("Must analyze file first.");
                }
                return recordCount;
            }
            /// <summary>
            /// Set the record count.
            /// </summary>
            ///
            /// <param name="v">The record count.</param>
            set { recordCount = value; }
        }


        /// <summary>
        /// Set the status reporting object.
        /// </summary>
        ///
        /// <value>The status reporting object.</value>
        public IStatusReportable Report
        {
            /// <returns>The status reporting object.</returns>
            get { return report; }
            /// <summary>
            /// Set the status reporting object.
            /// </summary>
            ///
            /// <param name="theReport">The status reporting object.</param>
            set { report = value; }
        }


        /// <summary>
        /// Set the reporting interval.
        /// </summary>
        ///
        /// <value>The new reporting interval.</value>
        public int ReportInterval
        {
            /// <returns>The reporting interval, an update will be sent for every block of
            /// rows that matches the size of this property.</returns>
            get { return reportInterval; }
            /// <summary>
            /// Set the reporting interval.
            /// </summary>
            ///
            /// <param name="theReportInterval">The new reporting interval.</param>
            set { reportInterval = value; }
        }


        /// <summary>
        /// Set to true, if the file has been analyzed.
        /// </summary>
        ///
        /// <value>True, if the file has been analyzed.</value>
        public bool Analyzed
        {
            /// <returns>Has the file been analyzed.</returns>
            get { return analyzed; }
            /// <summary>
            /// Set to true, if the file has been analyzed.
            /// </summary>
            ///
            /// <param name="theAnalyzed">True, if the file has been analyzed.</param>
            set { analyzed = value; }
        }


        /// <summary>
        /// Set the flag to determine if we are expecting input headers.
        /// </summary>
        ///
        /// <value>Are input headers expected?</value>
        public bool ExpectInputHeaders
        {
            /// <returns>True if we are expecting input headers.</returns>
            get { return expectInputHeaders; }
            /// <summary>
            /// Set the flag to determine if we are expecting input headers.
            /// </summary>
            ///
            /// <param name="theExpectInputHeaders">Are input headers expected?</param>
            set { expectInputHeaders = value; }
        }


        /// <value>the produceOutputHeaders to set</value>
        public bool ProduceOutputHeaders
        {
            /// <returns>the produceOutputHeaders</returns>
            get { return produceOutputHeaders; }
            /// <param name="theProduceOutputHeaders">the produceOutputHeaders to set</param>
            set { produceOutputHeaders = value; }
        }

        /// <value>the script to set</value>
        public AnalystScript Script { /// <returns>the script</returns>
            get; /// <param name="theScript">the script to set</param>
            set; }

        #region QuantTask Members

        /// <summary>
        /// Request a stop.
        /// </summary>
        ///
        public void RequestStop()
        {
            cancel = true;
        }

        /// <returns>Should we stop?</returns>
        public bool ShouldStop()
        {
            return cancel;
        }

        #endregion

        /// <summary>
        /// Append a separator. The separator will only be appended if the line is
        /// not empty.  This is used to build comma(or other) separated lists.
        /// </summary>
        ///
        /// <param name="line">The line to append to.</param>
        /// <param name="format">The format to use.</param>
        public static void AppendSeparator(StringBuilder line,
                                           CSVFormat format)
        {
            if ((line.Length > 0)
                && !line.ToString().EndsWith(format.Separator + ""))
            {
                line.Append(format.Separator);
            }
        }


        /// <summary>
        /// Perform a basic analyze of the file. This method is used mostly
        /// internally.
        /// </summary>
        ///
        public void PerformBasicCounts()
        {
            if (outputFormat == null)
            {
                outputFormat = inputFormat;
            }

            ResetStatus();
            int rc = 0;
            var csv = new ReadCSV(inputFilename.ToString(),
                                  expectInputHeaders, inputFormat);
            while (csv.Next() && !cancel)
            {
                UpdateStatus(true);
                rc++;
            }
            recordCount = rc;
            columnCount = csv.ColumnNames.Count;

            ReadHeaders(csv);
            csv.Close();
            ReportDone(true);
        }

        /// <summary>
        /// Prepare the output file, write headers if needed.
        /// </summary>
        ///
        /// <param name="outputFile">The name of the output file.</param>
        /// <returns>The output stream for the text file.</returns>
        public StreamWriter PrepareOutputFile(FileInfo outputFile)
        {
            try
            {
                var tw = new StreamWriter(outputFile.OpenRead());
                if (outputFormat == null)
                {
                    outputFormat = inputFormat;
                }

                // write headers, if needed
                if (produceOutputHeaders)
                {
                    int index = 0;
                    var line = new StringBuilder();

                    if (inputHeadings != null)
                    {
                        foreach (String str  in  inputHeadings)
                        {
                            if (line.Length > 0)
                            {
                                line.Append(outputFormat.Separator);
                            }
                            line.Append("\"");
                            line.Append(str);
                            line.Append("\"");
                            index++;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < columnCount; i++)
                        {
                            line.Append("\"field:");
                            line.Append(i + 1);
                            line.Append("\"");
                        }
                    }
                    tw.WriteLine(line.ToString());
                }

                return tw;
            }
            catch (IOException e)
            {
                throw new QuantError(e);
            }
        }

        /// <summary>
        /// Read the headers from a CSV file. Used mostly internally.
        /// </summary>
        ///
        /// <param name="csv">The CSV file to read from.</param>
        public void ReadHeaders(ReadCSV csv)
        {
            if (expectInputHeaders)
            {
                inputHeadings = new String[csv.ColumnNames.Count];
                for (int i = 0; i < csv.ColumnNames.Count; i++)
                {
                    inputHeadings[i] = csv.ColumnNames[i];
                }
            }
            else
            {
                inputHeadings = new String[csv.ColumnNames.Count];

                int i_0 = 0;
                if (Script != null)
                {
                    foreach (DataField field  in  Script.Fields)
                    {
                        inputHeadings[i_0++] = field.Name;
                    }
                }

                while (i_0 < csv.ColumnNames.Count)
                {
                    inputHeadings[i_0] = "field:" + i_0;
                    i_0++;
                }
            }
        }

        /// <summary>
        /// Report that we are done. Used internally.
        /// </summary>
        ///
        /// <param name="isAnalyzing">True if we are analyzing.</param>
        public void ReportDone(bool isAnalyzing)
        {
            if (isAnalyzing)
            {
                report.Report(recordCount, recordCount,
                              "Done analyzing");
            }
            else
            {
                report.Report(recordCount, recordCount,
                              "Done processing");
            }
        }

        /// <summary>
        /// Report that we are done. Used internally.
        /// </summary>
        ///
        /// <param name="task">The message.</param>
        public void ReportDone(String task)
        {
            report.Report(recordCount, recordCount, task);
        }

        /// <summary>
        /// Reset the reporting stats. Used internally.
        /// </summary>
        ///
        public void ResetStatus()
        {
            lastUpdate = 0;
            currentRecord = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" inputFilename=");
            result.Append(inputFilename);
            result.Append(", recordCount=");
            result.Append(recordCount);
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        /// Update the status. Used internally.
        /// </summary>
        ///
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
        /// Report the current status.
        /// </summary>
        ///
        /// <param name="task">The string to report.</param>
        public void UpdateStatus(String task)
        {
            bool shouldDisplay = false;

            if (currentRecord == 0)
            {
                shouldDisplay = true;
            }

            currentRecord++;
            lastUpdate++;

            if (lastUpdate > reportInterval)
            {
                lastUpdate = 0;
                shouldDisplay = true;
            }

            if (shouldDisplay)
            {
                report.Report(recordCount, currentRecord, task);
            }
        }

        /// <summary>
        /// Validate that the file has been analyzed. Throw an error, if it has not.
        /// </summary>
        ///
        public void ValidateAnalyzed()
        {
            if (!analyzed)
            {
                throw new QuantError("File must be analyzed first.");
            }
        }

        /// <summary>
        /// Write a row to the output file.
        /// </summary>
        ///
        /// <param name="tw">The output stream.</param>
        /// <param name="row">The row to write out.</param>
        public void WriteRow(StreamWriter tw, LoadedRow row)
        {
            var line = new StringBuilder();

            for (int i = 0; i < row.Data.Length; i++)
            {
                AppendSeparator(line, outputFormat);
                line.Append(row.Data[i]);
            }

            tw.WriteLine(line.ToString());
        }
    }
}