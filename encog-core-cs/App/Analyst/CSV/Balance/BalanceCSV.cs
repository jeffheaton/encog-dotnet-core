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

namespace Encog.App.Analyst.CSV.Balance
{
    /// <summary>
    ///     Balance a CSV file. This utility is useful when you have several an
    ///     unbalanced training set. You may have a large number of one particular class,
    ///     and many fewer elements of other classes. This can hinder many Machine
    ///     Learning methods. This class can be used to balance the data.
    ///     Obviously this class cannot generate data. You must request how many items
    ///     you want per class. Some classes will have lower than this number if they
    ///     were already below the specified amount. Any class above this amount will be
    ///     trimmed to that amount.
    /// </summary>
    public class BalanceCSV : BasicFile
    {
        /// <summary>
        ///     Tracks the counts of each class.
        /// </summary>
        private IDictionary<String, Int32> _counts;

        /// <value>Tracks the counts of each class.</value>
        public IDictionary<String, Int32> Counts
        {
            get { return _counts; }
        }

        /// <summary>
        ///     Analyze the data. This counts the records and prepares the data to be
        ///     processed.
        /// </summary>
        /// <param name="inputFile">The input file to process.</param>
        /// <param name="headers">True, if headers are present.</param>
        /// <param name="format">The format of the CSV file.</param>
        public void Analyze(FileInfo inputFile, bool headers,
                            CSVFormat format)
        {
            InputFilename = inputFile;
            ExpectInputHeaders = headers;
            Format = format;

            Analyzed = true;

            PerformBasicCounts();
        }

        /// <summary>
        ///     Return a string that lists the counts per class.
        /// </summary>
        /// <returns>The counts per class.</returns>
        public String DumpCounts()
        {
            var result = new StringBuilder();

            foreach (String key  in _counts.Keys)
            {
                result.Append(key);
                result.Append(" : ");
                result.Append((_counts[key]));
                result.Append("\n");
            }
            return result.ToString();
        }


        /// <summary>
        ///     Process and balance the data.
        /// </summary>
        /// <param name="outputFile">The output file to write data to.</param>
        /// <param name="targetField"></param>
        /// <param name="countPer">The desired count per class.</param>
        public void Process(FileInfo outputFile, int targetField,
                            int countPer)
        {
            ValidateAnalyzed();
            StreamWriter tw = PrepareOutputFile(outputFile);

            _counts = new Dictionary<String, Int32>();

            var csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, Format);

            ResetStatus();
            while (csv.Next() && !ShouldStop())
            {
                var row = new LoadedRow(csv);
                UpdateStatus(false);
                String key = row.Data[targetField];
                int count;
                if (!_counts.ContainsKey(key))
                {
                    count = 0;
                }
                else
                {
                    count = _counts[key];
                }

                if (count < countPer)
                {
                    WriteRow(tw, row);
                    count++;
                }

                _counts[key] = count;
            }
            ReportDone(false);
            csv.Close();
            tw.Close();
        }
    }
}
