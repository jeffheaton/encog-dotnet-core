// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
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
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData.Temporal;
using Encog.Neural.Activation;

namespace Encog.Neural.NeuralData.Market
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

        /// <summary>
        /// The ticker symbol.
        /// </summary>
        public TickerSymbol Ticker
        {
            get
            {
                return ticker;
            }
        }

        /// <summary>
        /// The data type that this is.
        /// </summary>
        public MarketDataType DataType
        {
            get
            {
                return dataType;
            }
        }

    }
}
