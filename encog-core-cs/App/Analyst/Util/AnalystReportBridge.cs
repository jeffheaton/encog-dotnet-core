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
        private readonly EncogAnalyst _analyst;

        /// <summary>
        /// Construct the bridge object.
        /// </summary>
        ///
        /// <param name="theAnalyst">The Encog analyst to use.</param>
        public AnalystReportBridge(EncogAnalyst theAnalyst)
        {
            _analyst = theAnalyst;
        }

        #region IStatusReportable Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Report(int total, int current,
                           String message)
        {
            foreach (IAnalystListener listener  in  _analyst.Listeners)
            {
                listener.Report(total, current, message);
            }
        }

        #endregion
    }
}