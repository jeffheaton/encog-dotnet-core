using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data.Versatile.Columns;

namespace Encog.ML.Data.Versatile.Missing
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMissingHandler
    {
        /// <summary>
        /// Called by the normalizer to setup this handler.
        /// </summary>
        /// <param name="normalizationHelper">The normalizer that is being used.</param>
        void Init(NormalizationHelper normalizationHelper);

        /// <summary>
        /// Process a column's missing data.
        /// </summary>
        /// <param name="colDef">The column that is missing.</param>
        /// <returns>The value to use.</returns>
        String ProcessString(ColumnDefinition colDef);

        /// <summary>
        /// Process a column's missing data.
        /// </summary>
        /// <param name="colDef">The column that is missing.</param>
        /// <returns>The value to use.</returns>
        double ProcessDouble(ColumnDefinition colDef);
    }
}
