using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Basic;
using Encog.Util.CSV;
using System.IO;

namespace Encog.App.Quant.Filter
{
    /// <summary>
    /// This class can be used to remove certian rows from a CSV.  You can remove rows
    /// where a specific field has a specific value.
    /// </summary>
    public class FilterCSV : BasicFile
    {
        /// <summary>
        /// A list of the fields and their values, that should be excluded.
        /// </summary>
        public IList<ExcludedField> Excluded
        {
            get
            {
                return this.excludedFields;
            }
        }

        /// <summary>
        /// A count of the filtered rows.  This is the resulting line count for the output CSV.
        /// </summary>
        public int FilteredRowCount
        {
            get
            {
                return this.filteredCount;
            }
        }

        /// <summary>
        /// The excluded fields.
        /// </summary>
        private IList<ExcludedField> excludedFields = new List<ExcludedField>();

        /// <summary>
        /// A count of the filtered rows.
        /// </summary>
        private int filteredCount;

        /// <summary>
        /// Exclude rows where the specified field has the specified value.
        /// </summary>
        /// <param name="fieldNumber">The field number.</param>
        /// <param name="fieldValue">The field value.</param>
        public void Exclude(int fieldNumber, String fieldValue)
        {
            this.excludedFields.Add(new ExcludedField(fieldNumber, fieldValue));
        }

        /// <summary>
        /// Analyze the file.
        /// </summary>
        /// <param name="inputFile">The name of the input file.</param>
        /// <param name="headers">True, if headers are expected.</param>
        /// <param name="format">The format.</param>
        public void Analyze(String inputFile, bool headers, CSVFormat format)
        {
            this.InputFilename = inputFile;
            this.ExpectInputHeaders = headers;
            this.InputFormat = format;

            this.Analyzed = true;

            PerformBasicCounts();
        }

        /// <summary>
        /// Determine if the specified row should be processed, or not.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>True, if the row should be processed.</returns>
        private bool ShouldProcess(LoadedRow row)
        {
            foreach(ExcludedField field in this.excludedFields)
            {
                if( row.Data[field.FieldNumber].Trim().Equals(field.FieldValue.Trim()) )
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Process the input file.
        /// </summary>
        /// <param name="outputFile">The output file to write to.</param>
        public void Process(String outputFile)
        {
            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);

            TextWriter tw = this.PrepareOutputFile(outputFile);
            this.filteredCount = 0;

            ResetStatus();
            while (csv.Next())
            {
                UpdateStatus(false);
                LoadedRow row = new LoadedRow(csv);
                if (ShouldProcess(row))
                {
                    WriteRow(tw, row);
                    this.filteredCount++;
                }
            }
            ReportDone(false);
            tw.Close();
            csv.Close();
        }
    }
}
