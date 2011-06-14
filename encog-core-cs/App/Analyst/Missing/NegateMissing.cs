using Encog.App.Analyst.Script.Normalize;

namespace Encog.App.Analyst.Missing
{
    /// <summary>
    /// Handle missing values by attempting to negate their effect.  The midpoint of 
    /// the normalized range of each value is used.  This is a zero for [-1,1] or 0.5 for [0,1].
    /// </summary>
    public class NegateMissing : IHandleMissingValues
    {
        #region IHandleMissingValues Members

        /// <inheritdoc/>
        public double[] HandleMissing(EncogAnalyst analyst, AnalystField stat)
        {
            var result = new double[stat.ColumnsNeeded];
            double n = stat.NormalizedHigh - (stat.NormalizedHigh - stat.NormalizedLow/2);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = n;
            }
            return result;
        }

        #endregion
    }
}