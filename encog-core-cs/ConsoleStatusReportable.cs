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

namespace Encog
{
    /// <summary>
    /// A simple status report that goes to the console.
    /// </summary>
    public class ConsoleStatusReportable : IStatusReportable
    {
        #region IStatusReportable Members

        /// <summary>
        /// Simply display any status reports.
        /// </summary>
        /// <param name="total">Total amount.</param>
        /// <param name="current">Current item.</param>
        /// <param name="message">Current message.</param>
        public void Report(int total, int current,
                           String message)
        {
            if (total == 0)
            {
                Console.WriteLine(current + " : " + message);
            }
            else
            {
                Console.WriteLine(current + "/" + total + " : " + message);
            }
        }

        #endregion
    }
}
