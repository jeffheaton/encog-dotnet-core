using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Data.Market.FinanceDataSet
{
    /// <summary>
    /// Various symbols used in finance.
    /// </summary>
    public class FinanceSymbols 
    {
        /// <summary>
        /// An instrument symbol.  Holds the exchange and the symbol.
        /// </summary>
        public class Instrument : TickerSymbol
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
            /// Gets or sets the size of the tick for this instrument.
            /// </summary>
            /// <value>
            /// The size of the tick.
            /// </value>
            public int TickSize { get; set; }

            /// <summary>
            /// Gets or sets the strike price.
            /// </summary>
            /// <value>
            /// The strike.
            /// </value>
            public double Strike { get; set; }

            /// <summary>
            /// Gets or sets the call put.
            /// if this is an option, set C for Calls , P for Puts.
            /// </summary>
            /// <value>
            /// The call put.
            /// </value>
            public string CallPut { get; set;}

            /// <summary>
            /// Gets or sets the implied vols.
            /// </summary>
            /// <value>
            /// The implied vols.
            /// </value>
            public double[] ImpliedVols { get; set;}

            /// <summary>
            /// Gets or sets the currency.
            /// </summary>
            /// <value>
            /// The currency.
            /// </value>
            public string Currency { get; set; }



            /// <summary>
            /// Construct a ticker symbol with no exchange.
            /// </summary>
            /// <param name="symbol">The ticker symbol</param>
            public Instrument(String symbol)
                : base(symbol)
            {
                _symbol = symbol;
                _exchange = null;
            }

            /// <summary>
            /// Construct a ticker symbol with exchange.
            /// </summary>
            /// <param name="symbol">The ticker symbol.</param>
            /// <param name="exchange">The exchange.</param>
            public Instrument(String symbol, String exchange): base(symbol,exchange)
            {
                _symbol = symbol;
                _exchange = exchange;
            }

            /// <summary>
            /// The stock symbol.
            /// </summary>
            public new String Symbol
            {
                get { return _symbol; }
            }

            /// <summary>
            /// The exchange that this stock is on.
            /// </summary>
            public new String Exchange
            {
                get { return _exchange; }
            }


            /// <summary>
            /// Determine if two ticker symbols equal each other.
            /// </summary>
            /// <param name="other">The other ticker symbol.</param>
            /// <returns>True if the two symbols equal.</returns>
            public bool Equals(Instrument other)
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

                return this.Exchange != null && other.Exchange.Equals(this.Exchange);
            }
        
        }

       
    
    }

}
