using System;
using Encog.ML.Data.Versatile.Columns;

namespace Encog.ML.Data.Versatile.Normalizers
{
    /// <summary>
    /// The normalizer interface defines how to normalize a column.  The source of the
    /// normalization can be either string or double.
    /// </summary>
    public interface INormalizer
    {
        /// <summary>
        /// Determine the normalized size of the specified column.
        /// </summary>
        /// <param name="colDef">The column to check.</param>
        /// <returns>The size of the column normalized.</returns>
        int OutputSize(ColumnDefinition colDef);

        /// <summary>
        ///  Normalize a column from a string. The output will go to an array, starting at outputColumn. 
        /// </summary>
        /// <param name="colDef">The column that is being normalized.</param>
        /// <param name="value">The value to normalize.</param>
        /// <param name="outputData">The array to output to.</param>
        /// <param name="outputIndex">The index to start at in outputData.</param>
        /// <returns>The new index (in outputData) that we've moved to.</returns>
        int NormalizeColumn(ColumnDefinition colDef, String value,
                double[] outputData, int outputIndex);

        /// <summary>
        /// Normalize a column from a double. The output will go to an array, starting at outputColumn. 
        /// </summary>
        /// <param name="colDef">The column that is being normalized.</param>
        /// <param name="value">The value to normalize.</param>
        /// <param name="outputData">The array to output to.</param>
        /// <param name="outputIndex">The index to start at in outputData.</param>
        /// <returns>The new index (in outputData) that we've moved to.</returns>
        int NormalizeColumn(ColumnDefinition colDef, double value,
                double[] outputData, int outputIndex);

        /// <summary>
        /// Denormalize a value. 
        /// </summary>
        /// <param name="colDef">The column to denormalize.</param>
        /// <param name="data">The data to denormalize.</param>
        /// <param name="dataIndex">The starting location inside data.</param>
        /// <returns>The denormalized value.</returns>
        String DenormalizeColumn(ColumnDefinition colDef, IMLData data,
                int dataIndex);
    }
}
