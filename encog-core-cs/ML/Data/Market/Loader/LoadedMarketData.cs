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

namespace Encog.ML.Data.Market.Loader
{
    /// <summary>
    /// Market data loaded from a source.
    /// </summary>
    public class LoadedMarketData : IComparable<LoadedMarketData>
    {
        /// <summary>
        /// The data that was collection for the sample date.
        /// </summary>
        private readonly IDictionary<MarketDataType, Double> _data;

        /// <summary>
        /// What is the ticker symbol for this data sample.
        /// </summary>
        private readonly TickerSymbol _ticker;

        /// <summary>
        /// Construct one sample of market data.
        /// </summary>
        /// <param name="when">When was this sample taken.</param>
        /// <param name="ticker">What is the ticker symbol for this data.</param>
        public LoadedMarketData(DateTime when, TickerSymbol ticker)
        {
            When = when;
            _ticker = ticker;
            _data = new Dictionary<MarketDataType, Double>();
        }

        /// <summary>
        /// When is this data from.
        /// </summary>
        public DateTime When { get; set; }

        /// <summary>
        /// The ticker symbol that this data was from.
        /// </summary>
        public TickerSymbol Ticker
        {
            get { return _ticker; }
        }

        /// <summary>
        /// The data that was downloaded.
        /// </summary>
        public IDictionary<MarketDataType, Double> Data
        {
            get { return _data; }
        }

        #region IComparable<LoadedMarketData> Members

        /// <summary>
        /// Compare this object with another of the same type.
        /// </summary>
        /// <param name="other">The other object to compare.</param>
        /// <returns>Zero if equal, greater or less than zero to indicate order.</returns>
        public int CompareTo(LoadedMarketData other)
        {
            return When.CompareTo(other.When);
        }

        #endregion

        /// <summary>
        /// Set the specified type of data.
        /// </summary>
        /// <param name="t">The type of data to set.</param>
        /// <param name="d">The value to set.</param>
        public void SetData(MarketDataType t, double d)
        {
            _data[t] = d;
        }

        /// <summary>
        /// Get the specified data type.
        /// </summary>
        /// <param name="t">The type of data to get.</param>
        /// <returns>The value.</returns>
        public double GetData(MarketDataType t)
        {
            return _data[t];
        }
    }
}
