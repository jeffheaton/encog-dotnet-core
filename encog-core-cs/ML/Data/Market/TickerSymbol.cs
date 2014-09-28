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

namespace Encog.ML.Data.Market
{
    /// <summary>
    /// A ticker symbol.  Holds the exchange and the symbol.
    /// </summary>
    public class TickerSymbol
    {
        /// <summary>
        /// The exchange.
        /// </summary>
        private readonly String _exchange;

        /// <summary>
        /// The ticker symbol.
        /// </summary>
        private readonly String _symbol;


        /// <summary>
        /// Construct a ticker symbol with no exchange.
        /// </summary>
        /// <param name="symbol">The ticker symbol</param>
        public TickerSymbol(String symbol)
        {
            _symbol = symbol;
            _exchange = null;
        }

        /// <summary>
        /// Construct a ticker symbol with exchange.
        /// </summary>
        /// <param name="symbol">The ticker symbol.</param>
        /// <param name="exchange">The exchange.</param>
        public TickerSymbol(String symbol, String exchange)
        {
            _symbol = symbol;
            _exchange = exchange;
        }

        /// <summary>
        /// The stock symbol.
        /// </summary>
        public String Symbol
        {
            get { return _symbol; }
        }

        /// <summary>
        /// The exchange that this stock is on.
        /// </summary>
        public String Exchange
        {
            get { return _exchange; }
        }


        /// <summary>
        /// Determine if two ticker symbols equal each other.
        /// </summary>
        /// <param name="other">The other ticker symbol.</param>
        /// <returns>True if the two symbols equal.</returns>
        public bool Equals(TickerSymbol other)
        {
            // if the symbols do not even match then they are not equal
            if (!other.Symbol.Equals(this.Symbol))
            {
                return false;
            }

            // if the symbols match then we need to compare the exchanges
            if (other.Exchange == null && other.Exchange == null)
            {
                return true;
            }

            if (other.Exchange == null || this.Exchange == null)
            {
                return false;
            }

            return other.Exchange.Equals(this.Exchange);
        }
    }
}
