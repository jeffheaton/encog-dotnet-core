using System;
using Encog.Util.CSV;

namespace Encog.App.Analyst.CSV.Basic
{
    /// <summary>
    /// A row of a CSV file loaded to memory. This class is used internally by many
    /// of the Encog quant classes.
    /// </summary>
    ///
    public class LoadedRow
    {
        /// <summary>
        /// The row data.
        /// </summary>
        ///
        private readonly String[] data;

        /// <summary>
        /// Load a row from the specified CSV file.
        /// </summary>
        ///
        /// <param name="csv">The CSV file to use.</param>
        public LoadedRow(ReadCSV csv) : this(csv, 0)
        {
        }

        /// <summary>
        /// Construct a loaded row.
        /// </summary>
        ///
        /// <param name="csv">The CSV file to use.</param>
        /// <param name="extra">The number of extra columns to add.</param>
        public LoadedRow(ReadCSV csv, int extra)
        {
            int count = csv.GetCount();
            data = new String[count + extra];
            for (int i = 0; i < count; i++)
            {
                data[i] = csv.Get(i);
            }
        }


        /// <value>The row data.</value>
        public String[] Data
        {
            /// <returns>The row data.</returns>
            get { return data; }
        }
    }
}