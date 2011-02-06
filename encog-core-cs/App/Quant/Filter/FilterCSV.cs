using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Basic;
using Encog.Util.CSV;
using System.IO;

namespace Encog.App.Quant.Filter
{
    public class FilterCSV : BasicFile
    {
        public IList<ExcludedField> Excluded
        {
            get
            {
                return this.excludedFields;
            }
        }

        public int FilteredRowCount
        {
            get
            {
                return this.filteredCount;
            }
        }

        private IList<ExcludedField> excludedFields = new List<ExcludedField>();
        private int filteredCount;

        public void Exclude(int fieldNumber, String fieldValue)
        {
            this.excludedFields.Add(new ExcludedField(fieldNumber, fieldValue));
        }

        public void Analyze(String inputFile, bool headers, CSVFormat format)
        {
            this.InputFilename = inputFile;
            this.ExpectInputHeaders = headers;
            this.InputFormat = format;

            this.Analyzed = true;

            PerformBasicCounts();
        }

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

        public void Process(String outputFile)
        {
            ReadCSV csv = new ReadCSV(this.InputFilename, this.ExpectInputHeaders, this.InputFormat);

            TextWriter tw = this.PrepareOutputFile(outputFile);
            this.filteredCount = 0;

            while (csv.Next())
            {
                LoadedRow row = new LoadedRow(csv);
                if (ShouldProcess(row))
                {
                    WriteRow(tw, row);
                    this.filteredCount++;
                }
            }
            tw.Close();
            csv.Close();
        }
    }
}
