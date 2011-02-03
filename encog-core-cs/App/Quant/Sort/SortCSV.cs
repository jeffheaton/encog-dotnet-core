using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.Collections;
using System.IO;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Sort
{
    public class SortCSV: BasicFile
    {
       
        public IList<SortedField> SortOrder
        {
            get
            {
                return this.sortOrder;
            }
        }

        private List<LoadedRow> data = new List<LoadedRow>();
        private IList<SortedField> sortOrder = new List<SortedField>();

        private void ReadInputFile(String inputFile, bool headers, CSVFormat format)
        {
            ReadCSV csv = new ReadCSV(inputFile, headers, format);
            while (csv.Next())
            {
                LoadedRow row = new LoadedRow(csv);
                this.data.Add(row);
            }

            if (headers)
            {
                this.InputHeadings = csv.ColumnNames.ToArray<String>();
            }

            csv.Close();            
        }

        private void SortData()
        {
            IComparer<LoadedRow> comp = new RowComparitor(this);
            this.data.Sort(comp);
        }

        private void WriteOutputFile(String outputFile, bool headers, CSVFormat format)
        {
            TextWriter tw = this.PrepareOutputFile(outputFile);

            bool[] nonNumeric = new bool[this.InputHeadings.Length];
            bool first = true;

            // write the file
            foreach (LoadedRow row in this.data)
            {
                // for the first row, determine types
                if (first)
                {
                    for (int i = 0; i < this.InputHeadings.Length; i++)
                    {
                        double d;
                        String str = row.Data[i];
                        nonNumeric[i] = !double.TryParse(str, out d);
                    }
                    first = false;
                }
                
                // write the row
                StringBuilder line = new StringBuilder();

                for (int i = 0; i < this.InputHeadings.Length; i++)
                {
                    if (i > 0)
                    {
                        line.Append(",");
                    }

                    if (nonNumeric[i])
                    {
                        line.Append("\"");
                        line.Append(row.Data[i]);
                        line.Append("\"");
                    }
                    else
                    {
                        line.Append(row.Data[i]);
                    }
                }

                tw.WriteLine(line.ToString());
            }

            // close the file
            tw.Close();
        }

        public void Process(String inputFile, String outputFile, bool headers, CSVFormat format)
        {
            ReadInputFile(inputFile,headers, format);
            SortData();
            WriteOutputFile(outputFile, headers, format);
        }
    }
}
