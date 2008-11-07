using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Data.Market.Loader
{
    interface IMarketLoader
    {
        /// <summary>
        /// Load the specified ticker symbol for the specified date.
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="dataNeeded">Which data is actually needed.</param>
        /// <param name="from">Beginning date for load.</param>
        /// <param name="to">Ending date for load.</param>
        /// <returns>A collection of LoadedMarketData objects that was loaded.</returns>
        ICollection<LoadedMarketData> Load(TickerSymbol ticker,
                IList<MarketDataType> dataNeeded, DateTime from, DateTime to);
    }
}
