using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData.Temporal;
using Encog.Neural.Activation;

namespace Encog.Neural.NeuralData.Market
{
    public class MarketDataDescription : TemporalDataDescription
    {


        /// <summary>
        /// The ticker symbol to be loaded.
        /// </summary>
        private TickerSymbol ticker;

        /// <summary>
        /// The type of data to be loaded from the specified ticker symbol.
        /// </summary>
        private MarketDataType dataType;

        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="activationFunction">The activation function to apply to this data, can be null.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public MarketDataDescription(TickerSymbol ticker,
                 MarketDataType dataType,
                 IActivationFunction activationFunction, bool input,
                 bool predict)
            : base(activationFunction, Type.PERCENT_CHANGE, input, predict)
        {

            this.ticker = ticker;
            this.dataType = dataType;
        }

        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public MarketDataDescription(TickerSymbol ticker,
                 MarketDataType dataType, bool input,
                 bool predict)
            : this(ticker, dataType, null, input, predict)
        {

        }

        public TickerSymbol Ticker
        {
            get
            {
                return ticker;
            }
        }

        public MarketDataType DataType
        {
            get
            {
                return dataType;
            }
        }

    }
}
