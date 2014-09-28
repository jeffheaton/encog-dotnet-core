using System;
using Encog.ML.Data.Versatile.Columns;

namespace Encog.ML.Data.Versatile.Missing
{
    /// <summary>
    /// Handle missing data by using the mean value of that column.
    /// </summary>
    public class MeanMissingHandler: IMissingHandler
    {
        /// <inheritdoc/>
        public void Init(NormalizationHelper normalizationHelper)
        {

        }

        /// <inheritdoc/>
        public String ProcessString(ColumnDefinition colDef)
        {
            throw new EncogError("The mean missing handler only accepts continuous numeric values.");
        }

        /// <inheritdoc/>
        public double ProcessDouble(ColumnDefinition colDef)
        {
            return colDef.Mean;
        }
    }
}
