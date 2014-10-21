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
using Encog.App.Analyst.Script.Normalize;

namespace Encog.App.Analyst.Missing
{
    /// <summary>
    ///     Handle missing values by attempting to negate their effect.  The midpoint of
    ///     the normalized range of each value is used.  This is a zero for [-1,1] or 0.5 for [0,1].
    /// </summary>
    public class NegateMissing : IHandleMissingValues
    {
        #region IHandleMissingValues Members

        /// <inheritdoc />
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
