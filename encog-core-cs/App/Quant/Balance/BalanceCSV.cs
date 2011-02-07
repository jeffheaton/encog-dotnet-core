using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Basic;
using Encog.Util.CSV;
using System.IO;

namespace Encog.App.Quant.Balance
{
    /// <summary>
    /// Balance a CSV file.  This utility is useful when you have several an unbalanced
    /// training set.  You may have a large number of one particular class, and many fewer
    /// elements of other classes.  This can hinder many Machine Learning methods.  This
    /// class can be used to balance the data.
    /// 
    /// Obviously this class cannot generate data.  You must request how many items you
    /// want per class.  Some classes will have lower than this number if they were already
    /// below the specified amount.  Any class above this amount will be trimmed to that
    /// amount.
    /// </summary>
    public class BalanceCSV : BasicFile
    {
        /// <summary>
        /// Tracks the counts of each class.
        /// </summary>
        private IDictionary<String, int> counts;

        /// <summary>
        /// Tracks the counts of each class.
        /// </summary>
        public IDictionary<String, int> Counts { get { return this.counts; } }

        /// <summary>
        /// Analyze the data.  This counts the records and prepares the data to be
        /// processed.
        /// </summary>
        /// <param name="inputFile">The input file to process.</param>
        /// <param name="headers">True, if headers are present.</param>
        /// <param name="format">The format of the CSV file.</param>
        public void Analyze(String inputFile, bool headers, CSVFormat format)
        {
            this.InputFilename = inputFile;
            this.ExpectInputHeaders = headers;
            this.InputFormat = format;

            this.Analyzed = true;

            PerformBasicCounts();
        }

        /// <summary>
        /// Process and balance the data.
        /// </summary>
        /// <param name="outputFile">The output file to write data to.</param>
        /// <param name="targetField">The field that is being balanced, 
        /// this field determines the classes.</param>
        /// <param name="countPer">The desired count per class.</param>
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

        /// <summary>
        /// Return a string that lists the counts per class.  
        /// </summary>
        /// <returns>The counts per class.</returns>
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
