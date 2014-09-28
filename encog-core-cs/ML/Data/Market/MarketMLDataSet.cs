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
using System;
using System.Collections.Generic;
using Encog.ML.Data.Market.Loader;
using Encog.ML.Data.Temporal;
using Encog.Util.Time;

namespace Encog.ML.Data.Market
{
    /// <summary>
    /// A data set that is designed to hold market data. This class is based on the
    /// TemporalNeuralDataSet.  This class is designed to load financial data from
    /// external sources.  This class is designed to track financial data across days.
    /// However, it should be usable with other levels of granularity as well. 
    /// </summary>
    public sealed class MarketMLDataSet : TemporalMLDataSet
    {
        /// <summary>
        /// The loader to use to obtain the data.
        /// </summary>
        private readonly IMarketLoader _loader;

        /// <summary>
        /// A map between the data points and actual data.
        /// </summary>
        private readonly IDictionary<Int64, TemporalPoint> _pointIndex =
            new Dictionary<Int64, TemporalPoint>();

        /// <summary>
        /// Construct a market data set object.
        /// </summary>
        /// <param name="loader">The loader to use to get the financial data.</param>
        /// <param name="inputWindowSize">The input window size, that is how many datapoints do we use to predict.</param>
        /// <param name="predictWindowSize">How many datapoints do we want to predict.</param>
        public MarketMLDataSet(IMarketLoader loader,Int64 inputWindowSize, Int64 predictWindowSize)
            : base((int)inputWindowSize, (int)predictWindowSize)
        {
            _loader = loader;
            SequenceGrandularity = TimeUnit.Days;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketMLDataSet"/> class.
        /// </summary>
        /// <param name="loader">The loader.</param>
        /// <param name="inputWindowSize">Size of the input window.</param>
        /// <param name="predictWindowSize">Size of the predict window.</param>
        /// <param name="unit">The time unit to use.</param>
        public MarketMLDataSet(IMarketLoader loader,  Int64 inputWindowSize, Int64 predictWindowSize, TimeUnit unit)
            : base((int)inputWindowSize, (int)predictWindowSize)
        {

            _loader = loader;
            SequenceGrandularity =unit;
        }

        /// <summary>
        /// The loader that is being used for this set.
        /// </summary>
        public IMarketLoader Loader
        {
            get { return _loader; }
        }

        /// <summary>
        /// Add one description of the type of market data that we are seeking at
        /// each datapoint.
        /// </summary>
        /// <param name="desc"></param>
        public override void AddDescription(TemporalDataDescription desc)
        {
            if (!(desc is MarketDataDescription))
            {
                throw new MarketError(
                    "Only MarketDataDescription objects may be used "
                    + "with the MarketMLDataSet container.");
            }
            base.AddDescription(desc);
        }


        /// <summary>
        /// Create a datapoint at the specified date.
        /// </summary>
        /// <param name="when">The date to create the point at.</param>
        /// <returns>Returns the TemporalPoint created for the specified date.</returns>
        public override TemporalPoint CreatePoint(DateTime when)
        {
            Int64 sequence = (Int64)GetSequenceFromDate(when);
            TemporalPoint result;

            if (_pointIndex.ContainsKey(sequence))
            {
                result = _pointIndex[sequence];
            }
            else
            {
                result = base.CreatePoint(when);
                _pointIndex[(int)result.Sequence] = result;
            }

            return result;
        }


        /// <summary>
        /// Load data from the loader.
        /// </summary>
        /// <param name="begin">The beginning date.</param>
        /// <param name="end">The ending date.</param>
        public void Load(DateTime begin, DateTime end)
        {
            // define the starting point if it is not already defined
            if (StartingPoint == DateTime.MinValue)
            {
                StartingPoint = begin;
            }

            // clear out any loaded points
            Points.Clear();

            // first obtain a collection of symbols that need to be looked up
            IDictionary<TickerSymbol, object> symbolSet = new Dictionary<TickerSymbol, object>();
            foreach (MarketDataDescription desc in Descriptions)
            {
                if (symbolSet.Count == 0)
                {
                    symbolSet[desc.Ticker] = null;
                }
                foreach (TickerSymbol ts in symbolSet.Keys)
                {
                    if (!ts.Equals(desc.Ticker))
                    {
                        symbolSet[desc.Ticker] = null;
                        break;
                    }
                }
            }

            // now loop over each symbol and load the data
            foreach (TickerSymbol symbol in symbolSet.Keys)
            {
                LoadSymbol(symbol, begin, end);
            }

            // resort the points
            SortPoints();
        }


        /// <summary>
        /// Load one point of market data.
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="point">The point to load at.</param>
        /// <param name="item">The item being loaded.</param>
        private void LoadPointFromMarketData(TickerSymbol ticker,
                                             TemporalPoint point, LoadedMarketData item)
        {
            foreach (TemporalDataDescription desc in Descriptions)
            {
                var mdesc = (MarketDataDescription) desc;

                if (mdesc.Ticker.Equals(ticker))
                {
                    point.Data[mdesc.Index] = item.Data[mdesc.DataType];
                }
            }
        }

        /// <summary>
        /// Load one ticker symbol.
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="from">Load data from this date.</param>
        /// <param name="to">Load data to this date.</param>
        private void LoadSymbol(TickerSymbol ticker, DateTime from,
                                DateTime to)
        {
            IList < MarketDataType > types = new List<MarketDataType>();
            foreach (MarketDataDescription desc in Descriptions)
            {
                if (desc.Ticker.Equals(ticker))
                {
                    types.Add(desc.DataType);   
                }
            }
            ICollection<LoadedMarketData> data = Loader.Load(ticker, types, from, to);         
            foreach (LoadedMarketData item in data)
            {
                TemporalPoint point = CreatePoint(item.When);

                LoadPointFromMarketData(ticker, point, item);
            }
        }
    }
}
