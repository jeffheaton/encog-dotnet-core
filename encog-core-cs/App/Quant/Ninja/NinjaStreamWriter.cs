using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Encog.Util.CSV;
using Encog.Util.Time;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Ninja
{
    /// <summary>
    /// Used internally to write to a Ninja trader import file.
    /// </summary>
    public class NinjaStreamWriter
    {
        /// <summary>
        /// The percision to use.
        /// </summary>
        public int Percision { get; set; }

        /// <summary>
        /// The columns to use.
        /// </summary>
        private IList<String> columns = new List<String>();

        /// <summary>
        /// The output file.
        /// </summary>
        private TextWriter tw;

        /// <summary>
        /// The filename to write to.
        /// </summary>
        private String filename;

        /// <summary>
        /// True, if headers are present.
        /// </summary>
        private bool headers;

        /// <summary>
        /// The format of the CSV file.
        /// </summary>
        private CSVFormat format;

        /// <summary>
        /// The output line, as it is built.
        /// </summary>
        private StringBuilder line;

        /// <summary>
        /// True, if columns were defined.
        /// </summary>
        private bool columnsDefined;

        /// <summary>
        /// Construct the object, and set the defaults.
        /// </summary>
        public NinjaStreamWriter()
        {
            Percision = 10;
            columnsDefined = false;
        }

        /// <summary>
        /// Open the file for output.
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
        /// Write the headers.
        /// </summary>
        private void WriteHeaders()
        {
            if (tw == null)
                throw new EncogError("Must open file first.");

            StringBuilder line = new StringBuilder();

            line.Append(FileData.DATE);
            line.Append(this.format.Separator);
            line.Append(FileData.TIME);

            foreach (String str in this.columns)
            {
                if (line.Length > 0)
                    line.Append(this.format.Separator);

                line.Append("\"");
                line.Append(str);
                line.Append("\"");
            }
            this.tw.WriteLine(line.ToString());
        }

        /// <summary>
        /// Close the file.
        /// </summary>
        public void Close()
        {
            if (tw == null)
                throw new EncogError("Must open file first.");
            tw.Close();
        }

        /// <summary>
        /// Begin a bar, for the specified date/time.
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
            line.Append(this.format.Separator);
            line.Append(NumericDateUtil.Time2Int(dt));
        }

        /// <summary>
        /// End the current bar.
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
        /// Store a column.
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
                line.Append(this.format.Separator);
            }

            line.Append(this.format.Format(d, Percision));

            if (!columnsDefined)
            {
                this.columns.Add(name);
            }
        }
    }
}
