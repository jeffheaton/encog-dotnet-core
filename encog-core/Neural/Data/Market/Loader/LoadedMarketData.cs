using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.NeuralData.Market.Loader
{
    public class LoadedMarketData
    {
        /// <summary>
        /// When was this data sample taken.
        /// </summary>
        private DateTime when;

        /// <summary>
        /// What is the ticker symbol for this data sample.
        /// </summary>
        private TickerSymbol ticker;

        /// <summary>
        /// The data that was collection for the sample date.
        /// </summary>
        private IDictionary<MarketDataType, Double> data;

        public void SetData(MarketDataType t, double d)
        {
            this.data[t] = d;
        }

        public double GetData(MarketDataType t)
        {
            return this.data[t];
        }

        public DateTime When
        {
            get
            {
                return this.when;
            }
            set
            {
                this.when = value;
            }
        }

        public TickerSymbol Ticker
        {
            get
            {
                return this.ticker;
            }
        }

        public IDictionary<MarketDataType, Double> Data
        {
            get
            {
                return this.data;
            }
        }


        /// <summary>
        /// Construct one sample of market data.
        /// </summary>
        /// <param name="when">When was this sample taken.</param>
        /// <param name="ticker">What is the ticker symbol for this data.</param>
        public LoadedMarketData(DateTime when, TickerSymbol ticker)
        {
            this.when = when;
            this.ticker = ticker;
            this.data = new Dictionary<MarketDataType, Double>();
        }
    }
}
