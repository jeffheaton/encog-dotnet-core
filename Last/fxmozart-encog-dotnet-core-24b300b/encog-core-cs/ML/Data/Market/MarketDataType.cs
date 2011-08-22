//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
        Quote
    }
}