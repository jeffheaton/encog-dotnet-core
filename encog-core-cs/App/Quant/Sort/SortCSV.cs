using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;
using System.Collections;
using System.IO;

namespace Encog.App.Quant.Sort
{
    public class SortCSV
    {
        public CSVFormat Format { get; set; }
        
        public IList<SortedField> SortOrder
        {
            get
            {
                return this.sortOrder;
            }
        }

        private List<LoadedRow> data = new List<LoadedRow>();
        private IList<SortedField> sortOrder = new List<SortedField>();
        private String[] headings;

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
                this.headings = csv.ColumnNames.ToArray<String>();
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
            TextWriter tw = new StreamWriter(outputFile);
             
            // write headers, if needed
            if (headers)
            {
                int index = 0;
                StringBuilder line = new StringBuilder();
                foreach (String str in this.headings)
                {
                    if (line.Length > 0)
                    {
                        line.Append(",");
                    }
                    line.Append("\"");
                    line.Append(this.headings[index++]);
                    line.Append("\"");
                }
                tw.WriteLine(line.ToString());
            }

            bool[] nonNumeric = new bool[this.headings.Length];
            bool first = true;

            // write the file
            foreach (LoadedRow row in this.data)
            {
                // for the first row, determine types
                if (first)
                {
                    for (int i = 0; i < this.headings.Length; i++)
                    {
                        double d;
                        String str = row.Data[i];
                        nonNumeric[i] = !double.TryParse(str, out d);
                    }
                    first = false;
                }
                
                // write the row
                StringBuilder line = new StringBuilder();

                for (int i = 0; i < this.headings.Length; i++)
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
