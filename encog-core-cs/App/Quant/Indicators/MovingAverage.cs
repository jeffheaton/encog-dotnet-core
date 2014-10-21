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
using System.Collections.Generic;
using Encog.App.Analyst.CSV.Basic;

namespace Encog.App.Quant.Indicators
{
    /// <summary>
    ///     A simple moving average.
    /// </summary>
    public class MovingAverage : Indicator
    {
        /// <summary>
        ///     The name of this indicator.
        /// </summary>
        public const String NAME = "MovAvg";

        /// <summary>
        ///     The number of periods in this indicator.
        /// </summary>
        private readonly int periods;

        /// <summary>
        ///     Construct this object.
        /// </summary>
        /// <param name="thePeriods">The number of periods in this indicator.</param>
        /// <param name="output">True, if this indicator is predicted.</param>
        public MovingAverage(int thePeriods, bool output) : base(NAME, false, output)
        {
            periods = thePeriods;
            Output = output;
        }


        /// <value>The number of periods in this indicator.</value>
        public override int Periods
        {
            get { return periods; }
        }

        /// <summary>
        ///     Calculate this indicator.
        /// </summary>
        /// <param name="data">The data to use.</param>
        /// <param name="length">The length to calculate over.</param>
        public override sealed void Calculate(IDictionary<String, BaseCachedColumn> data,
                                              int length)
        {
            Require(data, FileData.Close);

            double[] close = data[FileData.Close].Data;
            double[] output = Data;

            int lookbackTotal = (periods - 1);

            int start = lookbackTotal;
            if (start > (periods - 1))
            {
                return;
            }

            double periodTotal = 0;
            int trailingIdx = start - lookbackTotal;
            int i = trailingIdx;
            if (periods > 1)
            {
                while (i < start)
                {
                    periodTotal += close[i++];
                }
            }

            int outIdx = periods - 1;
            do
            {
                periodTotal += close[i++];
                double t = periodTotal;
                periodTotal -= close[trailingIdx++];
                output[outIdx++] = t/periods;
            } while (i < close.Length);

            BeginningIndex = periods - 1;
            EndingIndex = output.Length - 1;

            for (i = 0; i < periods - 1; i++)
            {
                output[i] = 0;
            }
        }
    }
}
