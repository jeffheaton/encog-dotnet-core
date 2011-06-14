using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Analyst.Script.Normalize;

namespace Encog.App.Analyst.Missing
{
    /// <summary>
    /// Defines a method for dealing with missing values.
    /// </summary>
    public interface IHandleMissingValues
    {
        /// <summary>
        /// Handle the missing values for a column.
        /// </summary>
        /// <param name="analyst">The analyst to use.</param>
        /// <param name="stat">The column we are handling missing values for.</param>
        /// <returns>Null if the row should be skipped, otherwise the list of values.</returns>
        double[] HandleMissing(EncogAnalyst analyst, AnalystField stat);
    }
}
