using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Temporal;
using Encog.ML.Data.Market.FinanceDataSet;
namespace Encog.ML.Data.Market.FinanceDataSet
{
    public class FinanceDataDescription  :TemporalDataDescription
    {
               /// <summary>
        /// The type of data to be loaded from the specified ticker symbol.
        /// </summary>
        private readonly FinanceDataTypes _dataType;

        /// <summary>
        /// The ticker symbol to be loaded.
        /// </summary>
        private readonly FinanceSymbols.Instrument _ticker;

        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="type">The normalization type.</param>
        /// <param name="activationFunction"> The activation function to apply to this data, can be null.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public FinanceDataDescription(FinanceSymbols.Instrument ticker,
                                     FinanceDataTypes dataType, Type type,
                                     IActivationFunction activationFunction, bool input,
                                     bool predict)
            : base(activationFunction, type, input, predict)
        {
            _ticker = ticker;
            _dataType = dataType;
        }


        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="type">The normalization type.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public FinanceDataDescription(FinanceSymbols.Instrument ticker,
                                     FinanceDataTypes dataType, Type type, bool input,
                                     bool predict)
            : this(ticker, dataType, type, null, input, predict)
        {
        }

        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public FinanceDataDescription(FinanceSymbols.Instrument ticker,
                                     FinanceDataTypes dataType, bool input,
                                     bool predict)
            : this(ticker, dataType, Type.PercentChange, null, input, predict)
        {

        }


        
        /// <summary>
        /// The ticker symbol.
        /// </summary>
        public FinanceSymbols.Instrument Ticker
        {
            get { return _ticker; }
        }

        /// <summary>
        /// The data type that this is.
        /// </summary>
        public FinanceDataTypes DataType
        {
            get { return _dataType; }
        }
    }
}
