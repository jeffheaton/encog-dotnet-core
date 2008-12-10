// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
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
