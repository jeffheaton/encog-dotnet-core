using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using Encog.App.Quant.Basic;
using System.IO;

namespace Encog.App.Quant.Ninja
{
    public class NinjaFileConvert: BasicCachedFile
    {
        public void Process(string target)
        {
            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);
            TextWriter tw = new StreamWriter(target);

            while (csv.Next())
            {
                StringBuilder line = new StringBuilder();

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

            csv.Close();
            tw.Close();
        }

        public override void Analyze(String input, bool headers, CSVFormat format)
        {
            base.Analyze(input, headers, format);
        }
    }
}
