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

namespace Encog.App.Quant.Indicators.Predictive
{
    /// <summary>
    ///     Get the best close.
    /// </summary>
    public class BestClose : Indicator
    {
        /// <summary>
        ///     The name of this indicator.
        /// </summary>
        public const String NAME = "PredictBestClose";

        /// <summary>
        ///     The number of periods this indicator is for.
        /// </summary>
        private readonly int periods;

        /// <summary>
        ///     Construct the object.
        /// </summary>
        /// <param name="thePeriods">The number of periods.</param>
        /// <param name="output">True, if this indicator is to be predicted.</param>
        public BestClose(int thePeriods, bool output) : base(NAME, false, output)
        {
            periods = thePeriods;
            Output = output;
        }


        /// <value>The number of periods.</value>
        public override int Periods
        {
            get { return periods; }
        }

        /// <summary>
        ///     Calculate the indicator.
        /// </summary>
        /// <param name="data">The data available to the indicator.</param>
        /// <param name="length">The length available to the indicator.</param>
        public override sealed void Calculate(IDictionary<String, BaseCachedColumn> data,
                                              int length)
        {
            double[] close = data[FileData.Close].Data;
            double[] output = Data;

            int stop = length - periods;
            for (int i = 0; i < stop; i++)
            {
                double bestClose = Double.MinValue;
                for (int j = 1; j <= periods; j++)
                {
                    bestClose = Math.Max(close[i + j], bestClose);
                }
                output[i] = bestClose;
            }

            for (int i_0 = length - periods; i_0 < length; i_0++)
            {
                output[i_0] = 0;
            }

            BeginningIndex = 0;
            EndingIndex = length - periods - 1;
        }
    }
}
