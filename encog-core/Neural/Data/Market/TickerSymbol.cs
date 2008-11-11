using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.NeuralData.Market
{
    public class TickerSymbol
    {
        /// <summary>
        /// The ticker symbol.
        /// </summary>
        private String symbol;

        /// <summary>
        /// The exchange.
        /// </summary>
        private String exchange;

        public String Symbol
        {
            get
            {
                return this.symbol;
            }
        }

        public String Exchange
        {
            get
            {
                return this.exchange;
            }
        }


        /// <summary>
        /// Construct a ticker symbol with no exchange.
        /// </summary>
        /// <param name="String">The ticker symbol</param>
        public TickerSymbol(String symbol)
        {
            this.symbol = symbol;
            this.exchange = null;
        }

        /// <summary>
        /// Construct a ticker symbol with exchange.
        /// </summary>
        /// <param name="symbol">The ticker symbol.</param>
        /// <param name="exchange">The exchange.</param>
        public TickerSymbol(String symbol, String exchange)
        {
            this.symbol = symbol;
            this.exchange = exchange;
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
