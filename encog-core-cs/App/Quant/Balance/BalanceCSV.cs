using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Basic;
using Encog.Util.CSV;
using System.IO;

namespace Encog.App.Quant.Balance
{
    public class BalanceCSV : BasicFile
    {
        private IDictionary<String, int> counts;

        public IDictionary<String, int> Counts { get { return this.counts; } }

        public void Analyze(String inputFile, bool headers, CSVFormat format)
        {
            this.InputFilename = inputFile;
            this.ExpectInputHeaders = headers;
            this.InputFormat = format;

            this.Analyzed = true;

            PerformBasicCounts();
        }

        public void Process(String outputFile, int targetField, int countPer)
        {
            ValidateAnalyzed();
            TextWriter tw = this.PrepareOutputFile(outputFile);
            
            counts = new Dictionary<String, int>();

            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);

            ResetStatus();
            while (csv.Next())
            {
                LoadedRow row = new LoadedRow(csv);
                UpdateStatus(false);
                String key = row.Data[targetField];
                int count;
                if (!counts.ContainsKey(key))
                {
                    count = 1;
                }
                else
                {
                    count = counts[key];
                }

                if (count < countPer)
                {
                    WriteRow(tw, row);
                    count++;
                }

                counts[key] = count;
            }
            ReportDone(false);
            csv.Close();
            tw.Close();
        }

        public String DumpCounts()
        {
            StringBuilder result = new StringBuilder();
            foreach (String key in this.counts.Keys)
            {
                result.Append(key);
                result.Append(" : ");
                result.Append(this.counts[key]);
                result.Append("\n");
            }
            return result.ToString();
        }

    }
}
