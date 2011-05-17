using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Encog.App.Analyst.CSV.Basic;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV.Balance
{
    /// <summary>
    /// Balance a CSV file. This utility is useful when you have several an
    /// unbalanced training set. You may have a large number of one particular class,
    /// and many fewer elements of other classes. This can hinder many Machine
    /// Learning methods. This class can be used to balance the data.
    /// Obviously this class cannot generate data. You must request how many items
    /// you want per class. Some classes will have lower than this number if they
    /// were already below the specified amount. Any class above this amount will be
    /// trimmed to that amount.
    /// </summary>
    ///
    public class BalanceCSV : BasicFile
    {
        /// <summary>
        /// Tracks the counts of each class.
        /// </summary>
        ///
        private IDictionary<String, Int32> counts;

        /// <value>Tracks the counts of each class.</value>
        public IDictionary<String, Int32> Counts
        {
            /// <returns>Tracks the counts of each class.</returns>
            get { return counts; }
        }

        /// <summary>
        /// Analyze the data. This counts the records and prepares the data to be
        /// processed.
        /// </summary>
        ///
        /// <param name="inputFile">The input file to process.</param>
        /// <param name="headers">True, if headers are present.</param>
        /// <param name="format">The format of the CSV file.</param>
        public void Analyze(FileInfo inputFile, bool headers,
                            CSVFormat format)
        {
            InputFilename = inputFile;
            ExpectInputHeaders = headers;
            InputFormat = format;

            Analyzed = true;

            PerformBasicCounts();
        }

        /// <summary>
        /// Return a string that lists the counts per class.
        /// </summary>
        ///
        /// <returns>The counts per class.</returns>
        public String DumpCounts()
        {
            var result = new StringBuilder();

            foreach (String key  in counts.Keys)
            {
                result.Append(key);
                result.Append(" : ");
                result.Append((counts[key]));
                result.Append("\n");
            }
            return result.ToString();
        }


        /// <summary>
        /// Process and balance the data.
        /// </summary>
        ///
        /// <param name="outputFile">The output file to write data to.</param>
        /// <param name="targetField"></param>
        /// <param name="countPer">The desired count per class.</param>
        public void Process(FileInfo outputFile, int targetField,
                            int countPer)
        {
            ValidateAnalyzed();
            StreamWriter tw = PrepareOutputFile(outputFile);

            counts = new Dictionary<String, Int32>();

            var csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, InputFormat);

            ResetStatus();
            while (csv.Next() && !ShouldStop())
            {
                var row = new LoadedRow(csv);
                UpdateStatus(false);
                String key = row.Data[targetField];
                int count;
                if (!counts.ContainsKey(key))
                {
                    count = 0;
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
    }
}