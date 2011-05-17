using System;

namespace Encog.App.Analyst.Util
{
    /// <summary>
    /// Used to bridge the AnalystListerner to an StatusReportable object.
    /// </summary>
    ///
    public class AnalystReportBridge : IStatusReportable
    {
        /// <summary>
        /// The analyst to bridge to.
        /// </summary>
        ///
        private readonly EncogAnalyst analyst;

        /// <summary>
        /// Construct the bridge object.
        /// </summary>
        ///
        /// <param name="theAnalyst">The Encog analyst to use.</param>
        public AnalystReportBridge(EncogAnalyst theAnalyst)
        {
            analyst = theAnalyst;
        }

        #region IStatusReportable Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Report(int total, int current,
                           String message)
        {
            foreach (AnalystListener listener  in  analyst.Listeners)
            {
                listener.Report(total, current, message);
            }
        }

        #endregion
    }
}