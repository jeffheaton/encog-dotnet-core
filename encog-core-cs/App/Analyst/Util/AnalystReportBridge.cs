//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;

namespace Encog.App.Analyst.Util
{
    /// <summary>
    ///     Used to bridge the AnalystListerner to an StatusReportable object.
    /// </summary>
    public class AnalystReportBridge : IStatusReportable
    {
        /// <summary>
        ///     The analyst to bridge to.
        /// </summary>
        private readonly EncogAnalyst _analyst;

        /// <summary>
        ///     Construct the bridge object.
        /// </summary>
        /// <param name="theAnalyst">The Encog analyst to use.</param>
        public AnalystReportBridge(EncogAnalyst theAnalyst)
        {
            _analyst = theAnalyst;
        }

        #region IStatusReportable Members

        /// <summary>
        /// </summary>
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
