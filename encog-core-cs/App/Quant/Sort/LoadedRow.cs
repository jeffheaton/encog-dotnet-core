using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;

namespace Encog.App.Quant.Sort
{
    public class LoadedRow
    {
        private String[] data;

        public String[] Data { get { return this.data; } }

        public LoadedRow(ReadCSV csv)
        {
            int count = csv.GetColumnCount();
            this.data = new String[count];
            for (int i = 0; i < count; i++)
            {
                this.data[i] = csv.Get(i); 
            }
        }
    }
}
