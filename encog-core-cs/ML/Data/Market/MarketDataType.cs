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
namespace Encog.ML.Data.Market
{
    /// <summary>
    /// The types of market data that can be used.
    /// </summary>
    public enum MarketDataType
    {
        /// <summary>
        /// The market open for the day.
        /// </summary>
        Open,

        /// <summary>
        /// The market close for the day.
        /// </summary>
        Close,

        /// <summary>
        /// The volume for the day.
        /// </summary>
        Volume,

        /// <summary>
        /// The adjusted close.  Adjusted for splits and dividends.
        /// </summary>
        AdjustedClose,

        /// <summary>
        /// The high for the day.
        /// </summary>
        High,

        /// <summary>
        /// The low for the day.
        /// </summary>
        Low,

        /// <summary>
        /// A trade (Tick data).
        /// </summary>
        Trade,

              /// <summary>
        /// A quote (bid /ask)
        /// </summary>
        Quote,

        /// <summary>
        /// The bid from a quote
        /// </summary>
        Bid,

        /// <summary>
        /// The ask price from a quote
        /// </summary>
        Ask,

        /// <summary>
        /// the bid volume from a quote.
        /// </summary>
        BidSize,

        /// <summary>
        /// the ask size from a quote.
        /// </summary>
        AskSize,


        /// <summary>
        /// Range from Open to Close (Absolute).
        /// </summary>
        RangeOpenClose,

        /// <summary>
        /// Rangr from High to Low.
        /// </summary>
        RangeHighLow,

        /// <summary>
        /// Range Open to Close not using absolute numbers (No Math.Abs(Open - Close)) , this gives a directional range.
        /// </summary>
        RangeOpenCloseNonAbsolute,

        /// <summary>
        /// Percentage moves from one bar to the next.
        /// </summary>
        PercentageMove,

        /// <summary>
        /// The weighted prices. ( High + Low + 2 * Close) /4.
        /// </summary>
        Weighted,


        /// <summary>
        /// The median of closing prices (high + low) /2
        /// </summary>
        Median,

    
    }
}
