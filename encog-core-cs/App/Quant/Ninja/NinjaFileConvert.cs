using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using Encog.App.Quant.Basic;
using System.IO;

namespace Encog.App.Quant.Ninja
{
    /// <summary>
    /// A simple class to convert financial data files into the format used by NinjaTrader for
    /// input.
    /// </summary>
    public class NinjaFileConvert: BasicCachedFile
    {
        /// <summary>
        /// Process the file and output to the target file.
        /// </summary>
        /// <param name="target">The target file to write to.</param>
        public void Process(string target)
        {
            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            TextWriter tw = new StreamWriter(target);

            ResetStatus();
            while (csv.Next())
            {
                StringBuilder line = new StringBuilder();
                UpdateStatus(false);
                line.Append(this.GetColumnData(FileData.DATE,csv));
                line.Append(" ");
                line.Append(this.GetColumnData(FileData.TIME, csv));
                line.Append(";");
                line.Append(InputFormat.Format(double.Parse(this.GetColumnData(FileData.OPEN, csv)),this.Precision));
                line.Append(";");
                line.Append(InputFormat.Format(double.Parse(this.GetColumnData(FileData.HIGH, csv)), this.Precision));
                line.Append(";");
                line.Append(InputFormat.Format(double.Parse(this.GetColumnData(FileData.LOW, csv)), this.Precision));
                line.Append(";");
                line.Append(InputFormat.Format(double.Parse(this.GetColumnData(FileData.CLOSE, csv)), this.Precision));
                line.Append(";");
                line.Append(InputFormat.Format(double.Parse(this.GetColumnData(FileData.VOLUME, csv)), this.Precision));

                tw.WriteLine(line.ToString());
            }
            ReportDone(false);
            csv.Close();
            tw.Close();
        }

        /// <summary>
        /// Analyze the input file.
        /// </summary>
        /// <param name="input">The name of the input file.</param>
        /// <param name="headers">True, if headers are present.</param>
        /// <param name="format">The format of the input file.</param>
        public override void Analyze(String input, bool headers, CSVFormat format)
        {
            base.Analyze(input, headers, format);
        }
    }
}
