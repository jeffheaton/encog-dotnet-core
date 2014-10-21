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
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Temporal;

namespace Encog.ML.Data.Market
{
    /// <summary>
    /// This class is used to describe the type of financial data that is needed.
    /// Each piece of data can be used for input, prediction or both. If used for
    /// input, it will be used as data to help predict. If used for prediction, it
    /// will be one of the values predicted. It is possible, and quite common, to use
    /// data from both input and prediction.
    /// </summary>
    public class MarketDataDescription : TemporalDataDescription
    {
        /// <summary>
        /// The type of data to be loaded from the specified ticker symbol.
        /// </summary>
        private readonly MarketDataType _dataType;

        /// <summary>
        /// The ticker symbol to be loaded.
        /// </summary>
        private readonly TickerSymbol _ticker;

        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="type">The normalization type.</param>
        /// <param name="activationFunction"> The activation function to apply to this data, can be null.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public MarketDataDescription(TickerSymbol ticker,
                                     MarketDataType dataType, Type type,
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
        public MarketDataDescription(TickerSymbol ticker,
                                     MarketDataType dataType, Type type, bool input,
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
        public MarketDataDescription(TickerSymbol ticker,
                                     MarketDataType dataType, bool input,
                                     bool predict)
            : this(ticker, dataType, Type.PercentChange, null, input, predict)
        {
        }

        /// <summary>
        /// The ticker symbol.
        /// </summary>
        public TickerSymbol Ticker
        {
            get { return _ticker; }
        }

        /// <summary>
        /// The data type that this is.
        /// </summary>
        public MarketDataType DataType
        {
            get { return _dataType; }
        }
    }
}
