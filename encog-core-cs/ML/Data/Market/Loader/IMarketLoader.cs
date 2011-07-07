//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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

namespace Encog.ML.Data.Market.Loader
{
    /// <summary>
    /// A market loader for financial information.
    /// </summary>
    public interface IMarketLoader
    {
        /// <summary>
        /// Load the specified ticker symbol for the specified date.
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="dataNeeded">Which data is actually needed.</param>
        /// <param name="from">Beginning date for load.</param>
        /// <param name="to">Ending date for load.</param>
        /// <returns>A collection of LoadedMarketData objects that was loaded.</returns>
        ICollection<LoadedMarketData> Load(TickerSymbol ticker,
                                           IList<MarketDataType> dataNeeded, DateTime from, DateTime to);
    }
}