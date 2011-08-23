using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Script;

namespace Encog.App.Analyst.Missing
{
    /// <summary>
    /// Handle missing values by inserting the mode for a class, and the mean for a number.
    /// </summary>
    public class MeanAndModeMissing: IHandleMissingValues
    {
        /// <inheritdoc/>
        public double[] HandleMissing(EncogAnalyst analyst, AnalystField stat)
        {
            // mode?
            if (stat.Classify)
            {
                var m = stat.DetermineMode(analyst);
                return stat.Encode(m);
            }
            // mean
            var df = analyst.Script.FindDataField(stat.Name);
            var result = new double[1];
            result[0] = df.Mean;
            return result;
        }
    }
}