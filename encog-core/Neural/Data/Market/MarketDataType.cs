using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Data.Market
{

        /// <summary>
        /// The types of market data that can be used.
        /// </summary>
        public enum MarketDataType
        {
            /// <summary>
            /// The market open for the day.
            /// </summary>
            OPEN,

            /// <summary>
            /// The market close for the day.
            /// </summary>
            CLOSE,

            /// <summary>
            /// The volume for the day.
            /// </summary>
            VOLUME,

            /// <summary>
            /// The adjusted close.  Adjusted for splits and dividends.
            /// </summary>
            ADJUSTED_CLOSE,

            /// <summary>
            /// The high for the day.
            /// </summary>
            HIGH,

            /// <summary>
            /// The low for the day.
            /// </summary>
            LOW

        }
    
}
