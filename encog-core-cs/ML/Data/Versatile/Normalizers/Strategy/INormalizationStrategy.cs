using Encog.ML.Data.Versatile.Columns;

namespace Encog.ML.Data.Versatile.Normalizers.Strategy
{
    /// <summary>
    /// Defines the interface to a normalization strategy.
    /// </summary>
    public interface INormalizationStrategy
    {
        /// <summary>
        /// Calculate how many elements a column will normalize into.
        /// </summary>
        /// <param name="colDef">The column definition.</param>
        /// <param name="isInput">True, if this is an input column.</param>
        /// <returns>The number of elements needed to normalize this column.</returns>
        int NormalizedSize(ColumnDefinition colDef, bool isInput);

        /// <summary>
        /// Normalize a column, with a string input. 
        /// </summary>
        /// <param name="colDef">The column definition.</param>
        /// <param name="isInput">True, if this is an input column.</param>
        /// <param name="value">The value to normalize.</param>
        /// <param name="outpuData">The output data.</param>
        /// <param name="outputColumn">The element to begin outputing to.</param>
        /// <returns>The new output element, advanced by the correct amount.</returns>
        int NormalizeColumn(ColumnDefinition colDef, bool isInput, string value,
                double[] outpuData, int outputColumn);

        
        /// <summary>
        /// Normalize a column, with a double input. 
        /// </summary>
        /// <param name="colDef">The column definition.</param>
        /// <param name="isInput">True, if this is an input column.</param>
        /// <param name="output">The value to normalize.</param>
        /// <param name="idx">The output data.</param>
        /// <returns>The new output element, advanced by the correct amount.</returns>
        string DenormalizeColumn(ColumnDefinition colDef, bool isInput, IMLData output,
                int idx);
        
        /// <summary>
        /// Normalize a column, with a double value. 
        /// </summary>
        /// <param name="colDef">The column definition.</param>
        /// <param name="isInput">True, if this is an input column.</param>
        /// <param name="value">The value to normalize.</param>
        /// <param name="outpuData">The output data.</param>
        /// <param name="outputColumn">The element to begin outputing to.</param>
        /// <returns>The new output element, advanced by the correct amount.</returns>
        int NormalizeColumn(ColumnDefinition colDef, bool isInput, double value,
                double[] outpuData, int outputColumn);
    }
}
