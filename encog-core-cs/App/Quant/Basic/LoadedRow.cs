using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CSV;

namespace Encog.App.Quant.Basic
{
    /// <summary>
    /// A row of a CSV file loaded to memory.  This class is used internally by
    /// many of the Encog quant classes.
    /// </summary>
    public class LoadedRow
    {
        /// <summary>
        /// The row data.
        /// </summary>
        private String[] data;

        /// <summary>
        /// The row data.
        /// </summary>
        public String[] Data { get { return this.data; } }

        /// <summary>
        /// Load a row from the specified CSV file.
        /// </summary>
        /// <param name="csv">The CSV file to use.</param>
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
