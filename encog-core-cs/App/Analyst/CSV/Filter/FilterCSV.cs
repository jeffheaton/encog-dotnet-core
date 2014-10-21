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
using System.Linq;
using Encog.App.Analyst.CSV.Basic;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV.Filter
{
    /// <summary>
    ///     This class can be used to remove certain rows from a CSV. You can remove rows
    ///     where a specific field has a specific value
    /// </summary>
    public class FilterCSV : BasicFile
    {
        /// <summary>
        ///     The excluded fields.
        /// </summary>
        private readonly IList<ExcludedField> _excludedFields;

        /// <summary>
        ///     A count of the filtered rows.
        /// </summary>
        private int _filteredCount;

        /// <summary>
        ///     Construct the object.
        /// </summary>
        public FilterCSV()
        {
            _excludedFields = new List<ExcludedField>();
        }


        /// <value>A list of the fields and their values, that should be excluded.</value>
        public IList<ExcludedField> Excluded
        {
            get { return _excludedFields; }
        }


        /// <value>
        ///     A count of the filtered rows. This is the resulting line count
        ///     for the output CSV.
        /// </value>
        public int FilteredRowCount
        {
            get { return _filteredCount; }
        }

        /// <summary>
        ///     Analyze the file.
        /// </summary>
        /// <param name="inputFile">The name of the input file.</param>
        /// <param name="headers">True, if headers are expected.</param>
        /// <param name="format">The format.</param>
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
        ///     Exclude rows where the specified field has the specified value.
        /// </summary>
        /// <param name="fieldNumber">The field number.</param>
        /// <param name="fieldValue">The field value.</param>
        public void Exclude(int fieldNumber, String fieldValue)
        {
            _excludedFields.Add(new ExcludedField(fieldNumber, fieldValue));
        }


        /// <summary>
        ///     Process the input file.
        /// </summary>
        /// <param name="outputFile">The output file to write to.</param>
        public void Process(FileInfo outputFile)
        {
            var csv = new ReadCSV(InputFilename.ToString(),
                                  ExpectInputHeaders, Format);

            StreamWriter tw = PrepareOutputFile(outputFile);
            _filteredCount = 0;

            ResetStatus();
            while (csv.Next() && !ShouldStop())
            {
                UpdateStatus(false);
                var row = new LoadedRow(csv);
                if (ShouldProcess(row))
                {
                    WriteRow(tw, row);
                    _filteredCount++;
                }
            }
            ReportDone(false);
            tw.Close();
            csv.Close();
        }

        /// <summary>
        ///     Determine if the specified row should be processed, or not.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>True, if the row should be processed.</returns>
        private bool ShouldProcess(LoadedRow row)
        {
            return _excludedFields.All(field => !row.Data[field.FieldNumber].Trim().Equals(field.FieldValue.Trim()));
        }
    }
}
