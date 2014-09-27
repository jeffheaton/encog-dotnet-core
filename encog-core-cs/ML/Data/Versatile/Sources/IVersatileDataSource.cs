using System;

namespace Encog.ML.Data.Versatile.Sources
{
    /// <summary>
    /// Defines a data source for the versatile data set.
    /// </summary>
    public interface IVersatileDataSource
    {
        /// <summary>
        /// Read a line from the source.
        /// </summary>
        /// <returns>The values read.</returns>
        String[] ReadLine();

        /// <summary>
        /// Rewind the source back to the beginning.
        /// </summary>
        void Rewind();

        /// <summary>
        /// Obtain the column index for the specified name.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <returns>The column index, or -1 if not found.</returns>
        int ColumnIndex(string name);
    }
}
