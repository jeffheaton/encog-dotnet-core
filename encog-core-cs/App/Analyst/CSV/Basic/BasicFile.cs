//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.IO;
using System.Text;
using Encog.App.Analyst.Script;
using Encog.App.Quant;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV.Basic
{
    /// <summary>
    ///     Many of the Encog quant CSV processors are based upon this class. This class
    ///     is not useful on its own. However, it does form the foundation for most Encog
    ///     CSV file processing.
    /// </summary>
    public class BasicFile : QuantTask
    {
        /// <summary>
        ///     The default report interval.
        /// </summary>
        private const int REPORT_INTERVAL = 10000;

        /// <summary>
        ///     Most Encog CSV classes must analyze a CSV file before actually processing
        ///     it. This property specifies if the file has been analyzed yet.
        /// </summary>
        private bool _analyzed;

        /// <summary>
        ///     True, if the process should stop.
        /// </summary>
        private bool _cancel;

        /// <summary>
        ///     The number of columns in the input file.
        /// </summary>
        private int _columnCount;

        /// <summary>
        ///     The current record.
        /// </summary>
        private int _currentRecord;

        /// <summary>
        ///     True, if input headers should be expected.
        /// </summary>
        private bool _expectInputHeaders;

        /// <summary>
        ///     The format of the input file.
        /// </summary>
        private CSVFormat _format;

        /// <summary>
        ///     The input filename. This is the file being analyzed/processed.
        /// </summary>
        private FileInfo _inputFilename;

        /// <summary>
        ///     The column headings from the input file.
        /// </summary>
        private String[] _inputHeadings;

        /// <summary>
        ///     The last time status was updated.
        /// </summary>
        private int _lastUpdate;

        /// <summary>
        ///     Should output headers be produced?
        /// </summary>
        private bool _produceOutputHeaders;

        /// <summary>
        ///     The number of records to process. This is determined when the file is
        ///     analyzed.
        /// </summary>
        private int _recordCount;

        /// <summary>
        ///     Allows status to be reported. Defaults to no status reported.
        /// </summary>
        private IStatusReportable _report;

        /// <summary>
        ///     The number of records to process before status is updated. Defaults to
        ///     10k.
        /// </summary>
        private int _reportInterval;

        /// <summary>
        ///     Construct the object, and set the defaults.
        /// </summary>
        public BasicFile()
        {
            Precision = EncogFramework.DefaultPrecision;
            _report = new NullStatusReportable();
            _reportInterval = REPORT_INTERVAL;
            _produceOutputHeaders = true;
            ResetStatus();
        }

        /// <summary>
        ///     Set the column count.
        /// </summary>
        public int Count
        {
            get { return _columnCount; }
            set { _columnCount = value; }
        }


        /// <summary>
        ///     Set the input filename.
        /// </summary>
        public FileInfo InputFilename
        {
            get { return _inputFilename; }
            set { _inputFilename = value; }
        }


        /// <summary>
        ///     Set the format.
        /// </summary>
        public CSVFormat Format
        {
            get { return _format; }
            set { _format = value; }
        }


        /// <summary>
        ///     Set the input headings.
        /// </summary>
        public String[] InputHeadings
        {
            get { return _inputHeadings; }
            set { _inputHeadings = value; }
        }

        /// <summary>
        ///     Set the precision to use.
        /// </summary>
        public int Precision { get; set; }


        /// <summary>
        ///     Set the record count.
        /// </summary>
        public int RecordCount
        {
            get
            {
                if (!_analyzed)
                {
                    throw new QuantError("Must analyze file first.");
                }
                return _recordCount;
            }
            set { _recordCount = value; }
        }


        /// <summary>
        ///     Set the status reporting object.
        /// </summary>
        public IStatusReportable Report
        {
            get { return _report; }
            set { _report = value; }
        }


        /// <summary>
        ///     Set the reporting interval.
        /// </summary>
        public int ReportInterval
        {
            get { return _reportInterval; }
            set { _reportInterval = value; }
        }


        /// <summary>
        ///     Set to true, if the file has been analyzed.
        /// </summary>
        public bool Analyzed
        {
            get { return _analyzed; }
            set { _analyzed = value; }
        }


        /// <summary>
        ///     Set the flag to determine if we are expecting input headers.
        /// </summary>
        public bool ExpectInputHeaders
        {
            get { return _expectInputHeaders; }
            set { _expectInputHeaders = value; }
        }


        /// <value>the produceOutputHeaders to set</value>
        public bool ProduceOutputHeaders
        {
            get { return _produceOutputHeaders; }
            set { _produceOutputHeaders = value; }
        }

        /// <value>the script to set</value>
        public AnalystScript Script { get; set; }

        #region QuantTask Members

        /// <summary>
        ///     Request a stop.
        /// </summary>
        public void RequestStop()
        {
            _cancel = true;
        }

        /// <returns>Should we stop?</returns>
        public bool ShouldStop()
        {
            return _cancel;
        }

        #endregion

        /// <summary>
        ///     Append a separator. The separator will only be appended if the line is
        ///     not empty.  This is used to build comma(or other) separated lists.
        /// </summary>
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
        ///     Perform a basic analyze of the file. This method is used mostly
        ///     internally.
        /// </summary>
        public void PerformBasicCounts()
        {
            ResetStatus();
            int rc = 0;
            var csv = new ReadCSV(_inputFilename.ToString(),
                                  _expectInputHeaders, _format);
            while (csv.Next() && !_cancel)
            {
                UpdateStatus(true);
                rc++;
            }
            _recordCount = rc;
            _columnCount = csv.ColumnCount;

            ReadHeaders(csv);
            csv.Close();
            ReportDone(true);
        }

        /// <summary>
        ///     Prepare the output file, write headers if needed.
        /// </summary>
        /// <param name="outputFile">The name of the output file.</param>
        /// <returns>The output stream for the text file.</returns>
        public StreamWriter PrepareOutputFile(FileInfo outputFile)
        {
            try
            {
                outputFile.Delete();
                var tw = new StreamWriter(outputFile.OpenWrite());

                // write headers, if needed
                if (_produceOutputHeaders)
                {
                    var line = new StringBuilder();

                    if (_inputHeadings != null)
                    {
                        foreach (String str  in  _inputHeadings)
                        {
                            if (line.Length > 0)
                            {
                                line.Append(_format.Separator);
                            }
                            line.Append("\"");
                            line.Append(str);
                            line.Append("\"");
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _columnCount; i++)
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
        ///     Read the headers from a CSV file. Used mostly internally.
        /// </summary>
        /// <param name="csv">The CSV file to read from.</param>
        public void ReadHeaders(ReadCSV csv)
        {
            if (_expectInputHeaders)
            {
                _inputHeadings = new String[csv.ColumnCount];
                for (int i = 0; i < csv.ColumnCount; i++)
                {
                    _inputHeadings[i] = csv.ColumnNames[i];
                }
            }
            else
            {
                _inputHeadings = new String[csv.ColumnCount];

                int i = 0;
                if (Script != null)
                {
                    foreach (DataField field  in  Script.Fields)
                    {
                        _inputHeadings[i++] = field.Name;
                    }
                }

                while (i < csv.ColumnCount)
                {
                    _inputHeadings[i] = "field:" + i;
                    i++;
                }
            }
        }

        /// <summary>
        ///     Report that we are done. Used internally.
        /// </summary>
        /// <param name="isAnalyzing">True if we are analyzing.</param>
        public void ReportDone(bool isAnalyzing)
        {
            _report.Report(_recordCount, _recordCount,
                           isAnalyzing ? "Done analyzing" : "Done processing");
        }

        /// <summary>
        ///     Report that we are done. Used internally.
        /// </summary>
        /// <param name="task">The message.</param>
        public void ReportDone(String task)
        {
            _report.Report(_recordCount, _recordCount, task);
        }

        /// <summary>
        ///     Reset the reporting stats. Used internally.
        /// </summary>
        public void ResetStatus()
        {
            _lastUpdate = 0;
            _currentRecord = 0;
        }


        /// <inheritdoc />
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" inputFilename=");
            result.Append(_inputFilename);
            result.Append(", recordCount=");
            result.Append(_recordCount);
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        ///     Update the status. Used internally.
        /// </summary>
        /// <param name="isAnalyzing">True if we are in the process of analyzing.</param>
        public void UpdateStatus(bool isAnalyzing)
        {
            UpdateStatus(isAnalyzing ? "Analyzing" : "Processing");
        }

        /// <summary>
        ///     Report the current status.
        /// </summary>
        /// <param name="task">The string to report.</param>
        public void UpdateStatus(String task)
        {
            bool shouldDisplay = false;

            if (_currentRecord == 0)
            {
                shouldDisplay = true;
            }

            _currentRecord++;
            _lastUpdate++;

            if (_lastUpdate >= _reportInterval)
            {
                _lastUpdate = 0;
                shouldDisplay = true;
            }

            if (shouldDisplay)
            {
                _report.Report(_recordCount, _currentRecord, task);
            }
        }

        /// <summary>
        ///     Validate that the file has been analyzed. Throw an error, if it has not.
        /// </summary>
        public void ValidateAnalyzed()
        {
            if (!_analyzed)
            {
                throw new QuantError("File must be analyzed first.");
            }
        }

        /// <summary>
        ///     Write a row to the output file.
        /// </summary>
        /// <param name="tw">The output stream.</param>
        /// <param name="row">The row to write out.</param>
        public void WriteRow(StreamWriter tw, LoadedRow row)
        {
            var line = new StringBuilder();

            foreach (string t in row.Data)
            {
                AppendSeparator(line, _format);
                line.Append(t);
            }

            tw.WriteLine(line.ToString());
        }
    }
}
