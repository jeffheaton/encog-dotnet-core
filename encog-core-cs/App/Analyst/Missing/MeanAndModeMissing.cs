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
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Script.Normalize;

namespace Encog.App.Analyst.Missing
{
    /// <summary>
    ///     Handle missing values by inserting the mode for a class, and the mean for a number.
    /// </summary>
    public class MeanAndModeMissing : IHandleMissingValues
    {
        /// <inheritdoc />
        public double[] HandleMissing(EncogAnalyst analyst, AnalystField stat)
        {
            // mode?
            if (stat.Classify)
            {
                int m = stat.DetermineMode(analyst);
                return stat.Encode(m);
            }
            // mean
            DataField df = analyst.Script.FindDataField(stat.Name);
            var result = new double[1];
            result[0] = df.Mean;
            return result;
        }
    }
}
