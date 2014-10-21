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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.Util.CSV;
using Encog.Util.Time;

namespace Encog.App.Quant.Ninja
{
    /// <summary>
    ///     Can be used from within NinjaTrader to export data.  This class is usually placed
    ///     inside of a NinjaTrader indicator to export NinjaTrader indicators and data.
    /// </summary>
    public class NinjaStreamWriter
    {
        /// <summary>
        ///     The columns to use.
        /// </summary>
        private readonly IList<String> columns = new List<String>();

        /// <summary>
        ///     True, if columns were defined.
        /// </summary>
        private bool columnsDefined;

        /// <summary>
        ///     The format of the CSV file.
        /// </summary>
        private CSVFormat format;

        /// <summary>
        ///     True, if headers are present.
        /// </summary>
        private bool headers;

        /// <summary>
        ///     The output line, as it is built.
        /// </summary>
        private StringBuilder line;

        /// <summary>
        ///     The output file.
        /// </summary>
        private TextWriter tw;

        /// <summary>
        ///     Construct the object, and set the defaults.
        /// </summary>
        public NinjaStreamWriter()
        {
            Percision = 10;
            columnsDefined = false;
        }

        /// <summary>
        ///     The percision to use.
        /// </summary>
        public int Percision { get; set; }

        /// <summary>
        ///     Open the file for output.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="headers">True, if headers are present.</param>
        /// <param name="format">The CSV format.</param>
        public void Open(String filename, bool headers, CSVFormat format)
        {
            tw = new StreamWriter(filename);
            this.format = format;
            this.headers = headers;
        }

        /// <summary>
        ///     Write the headers.
        /// </summary>
        private void WriteHeaders()
        {
            if (tw == null)
                throw new EncogError("Must open file first.");

            var line = new StringBuilder();

            line.Append(FileData.Date);
            line.Append(format.Separator);
            line.Append(FileData.Time);

            foreach (String str in columns)
            {
                if (line.Length > 0)
                    line.Append(format.Separator);

                line.Append("\"");
                line.Append(str);
                line.Append("\"");
            }
            tw.WriteLine(line.ToString());
        }

        /// <summary>
        ///     Close the file.
        /// </summary>
        public void Close()
        {
            if (tw == null)
                throw new EncogError("Must open file first.");
            tw.Close();
        }

        /// <summary>
        ///     Begin a bar, for the specified date/time.
        /// </summary>
        /// <param name="dt">The date/time where the bar begins.</param>
        public void BeginBar(DateTime dt)
        {
            if (tw == null)
            {
                throw new EncogError("Must open file first.");
            }

            if (line != null)
            {
                throw new EncogError("Must call end bar");
            }

            line = new StringBuilder();
            line.Append(NumericDateUtil.DateTime2Long(dt));
            line.Append(format.Separator);
            line.Append(NumericDateUtil.Time2Int(dt));
        }

        /// <summary>
        ///     End the current bar.
        /// </summary>
        public void EndBar()
        {
            if (tw == null)
            {
                throw new EncogError("Must open file first.");
            }

            if (line == null)
            {
                throw new EncogError("Must call BeginBar first.");
            }

            if (headers && !columnsDefined)
            {
                WriteHeaders();
            }

            tw.WriteLine(line.ToString());
            line = null;
            columnsDefined = true;
        }

        /// <summary>
        ///     Store a column.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="d">The value to store.</param>
        public void StoreColumn(String name, double d)
        {
            if (line == null)
            {
                throw new EncogError("Must call BeginBar first.");
            }

            if (line.Length > 0)
            {
                line.Append(format.Separator);
            }

            line.Append(format.Format(d, Percision));

            if (!columnsDefined)
            {
                columns.Add(name);
            }
        }
    }
}
