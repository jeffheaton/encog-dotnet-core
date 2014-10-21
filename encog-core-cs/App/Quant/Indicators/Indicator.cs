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
    ///     An indicator, used by Encog.
    /// </summary>
    public abstract class Indicator : BaseCachedColumn
    {
        /// <summary>
        ///     Construct the indicator.
        /// </summary>
        /// <param name="name">The indicator name.</param>
        /// <param name="input">Is this indicator used to predict?</param>
        /// <param name="output">Is this indicator what we are trying to predict.</param>
        public Indicator(String name, bool input,
                         bool output) : base(name, input, output)
        {
        }


        /// <value>the beginningIndex to set</value>
        public int BeginningIndex { get; set; }


        /// <value>the endingIndex to set.</value>
        public int EndingIndex { get; set; }


        /// <value>The number of periods this indicator is for.</value>
        public abstract int Periods { get; }

        /// <summary>
        ///     Calculate this indicator.
        /// </summary>
        /// <param name="data">The data available to this indicator.</param>
        /// <param name="length">The length of data to use.</param>
        public abstract void Calculate(IDictionary<String, BaseCachedColumn> data,
                                       int length);


        /// <summary>
        ///     Require a specific type of underlying data.
        /// </summary>
        /// <param name="theData">The data available.</param>
        /// <param name="item">The type of data we are looking for.</param>
        public void Require(IDictionary<String, BaseCachedColumn> theData,
                            String item)
        {
            if (!theData.ContainsKey(item))
            {
                throw new QuantError(
                    "To use this indicator, the underlying data must contain: "
                    + item);
            }
        }
    }
}
