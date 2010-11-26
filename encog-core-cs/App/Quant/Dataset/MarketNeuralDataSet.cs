// Encog(tm) Artificial Intelligence Framework v2.5
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
using Encog.Util.Time;
using Encog.App.Quant.MarketDB;

namespace Encog.App.Quant.Dataset
{
    /// <summary>
    /// A data set that is designed to hold market data. This class is based on the
    /// TemporalNeuralDataSet.  This class is designed to load financial data from
    /// external sources.  This class is designed to track financial data across days.
    /// However, it should be usable with other levels of granularity as well. 
    /// </summary>
    public class MarketNeuralDataSet : TemporalNeuralDataSet
    {
        /// <summary>
        /// The loader to use to obtain the data.
        /// </summary>
        private MarketDataStorage storage;

        /// <summary>
        /// The bar frequency.
        /// </summary>
        private BarPeriod frequency;

        /// <summary>
        /// A map between the data points and actual data.
        /// </summary>
        private IDictionary<long, TemporalPoint> pointIndex =
            new Dictionary<long, TemporalPoint>();

        /// <summary>
        /// Construct a market data set object.
        /// </summary>
        /// <param name="loader">The loader to use to get the financial data.</param>
        /// <param name="inputWindowSize">The input window size, that is how many datapoints do we use to predict.</param>
        /// <param name="predictWindowSize">How many datapoints do we want to predict.</param>
        /// <param name="frequency">The bar frequency to use (i.e. daily, hourly, minute, etc).</param>
        /// <param name="storage">The financial data store to use.</param>
        public MarketNeuralDataSet(MarketDataStorage storage,
                 int inputWindowSize, int predictWindowSize, BarPeriod frequency)
            : base(inputWindowSize, predictWindowSize)
        {

            this.storage = storage;
            this.frequency = frequency;
        }

        /// <summary>
        /// The bar frequency to use (i.e. daily, hourly, minute, etc).
        /// </summary>
        public BarPeriod BarFrequency
        {
            get
            {
                return this.frequency;
            }
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
                        + "with the MarketNeuralDataSet container.");
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
            long sequence = GetSequenceFromDate(when);
            TemporalPoint result = null;

            if (pointIndex.ContainsKey(sequence))
            {
                result = this.pointIndex[sequence];
            }
            else
            {
                result = base.CreatePoint(when);
                this.pointIndex[result.Sequence] = result;
            }

            return result;
        }

        /// <summary>
        /// The loader that is being used for this set.
        /// </summary>
        public MarketDataStorage Storage
        {
            get
            {
                return this.storage;
            }

        }


        /// <summary>
        /// Load data from the loader.
        /// </summary>
        /// <param name="begin">The beginning date.</param>
        /// <param name="end">The ending date.</param>
        public void Load(DateTime begin, DateTime end)
        {
            // define the starting point if it is not already defined
            if (this.StartingPoint == DateTime.MinValue)
            {
                this.StartingPoint = begin;
            }

            // clear out any loaded points
            this.Points.Clear();

            // first obtain a collection of symbols that need to be looked up
            IDictionary<String, object> set = new Dictionary<String, object>();
            foreach (TemporalDataDescription desc in this.Descriptions)
            {
                MarketDataDescription mdesc = (MarketDataDescription)desc;
                set[mdesc.Ticker] = null;
            }

            // now loop over each symbol and load the data
            foreach (String symbol in set.Keys)
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
        private void LoadPointFromMarketData(String ticker,
                 TemporalPoint point, StoredMarketData item)
        {
            foreach (TemporalDataDescription desc in this.Descriptions)
            {
                MarketDataDescription mdesc = (MarketDataDescription)desc;

                if (mdesc.Ticker.Equals(ticker))
                {                  
                    point.Data[mdesc.Index] = item.GetDataDouble(mdesc.DataType);
                }
            }
        }

        /// <summary>
        /// Load one ticker symbol.
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="from">Load data from this date.</param>
        /// <param name="to">Load data to this date.</param>
        private void LoadSymbol(String ticker, DateTime from,
                 DateTime to)
        {
            IList<StoredMarketData> data = this.storage.LoadRange(ticker, from, to, BarPeriod.EOD);
            foreach (StoredMarketData item in data)
            {
                TemporalPoint point = this.CreatePoint(item.Date);

                LoadPointFromMarketData(ticker, point, item);
            }
        }
    }
}
