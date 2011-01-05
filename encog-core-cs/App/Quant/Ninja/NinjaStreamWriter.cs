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
    public class NinjaStreamWriter
    {
        public int Percision { get; set; }

        private IList<String> columns = new List<String>();
        private TextWriter tw;
        private String filename;
        private bool headers;
        private CSVFormat format;
        private StringBuilder line;
        private bool columnsDefined;

        public NinjaStreamWriter()
        {
            Percision = 10;
            columnsDefined = false;
        }

        public void Open(String filename, bool headers, CSVFormat format)
        {
            tw = new StreamWriter(filename);
        }

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

        public void Close()
        {
            if (tw == null)
                throw new EncogError("Must open file first.");
            tw.Close();
        }

        public void BeginBar(DateTime dt)
        {
            if (tw == null)
            {
                throw new EncogError("Must open file first.");
            }

            if (line != null)
            {
                throw new EncogError("Must call begin bar");
            }

            line.Append(NumericDateUtil.DateTime2Long(dt));
            line.Append(this.format.Separator);
            line.Append(NumericDateUtil.Time2Int(dt));
        }

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
