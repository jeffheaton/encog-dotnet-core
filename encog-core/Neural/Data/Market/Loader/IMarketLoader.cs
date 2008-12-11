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

namespace Encog.Neural.NeuralData.Market.Loader
{
    /// <summary>
    /// A market loader for financial information.
    /// </summary>
    public interface IMarketLoader
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
