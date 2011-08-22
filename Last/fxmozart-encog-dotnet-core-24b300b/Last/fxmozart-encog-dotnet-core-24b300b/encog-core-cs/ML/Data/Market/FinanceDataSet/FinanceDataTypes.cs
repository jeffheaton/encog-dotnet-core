using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Data.Market.FinanceDataSet
{
    public class FinanceDataTypes
    {
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
            /// Range from High to Low.
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



        }
    }
}
