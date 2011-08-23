using Encog.App.Analyst.Script.Normalize;

namespace Encog.App.Analyst.Missing
{
    /// <summary>
    /// Handle missing values by discarding any rows that have missing values.
    /// </summary>
    public class DiscardMissing : IHandleMissingValues
    {
        #region IHandleMissingValues Members

        public double[] HandleMissing(EncogAnalyst analyst, AnalystField stat)
        {
            return null;
        }

        #endregion
    }
}