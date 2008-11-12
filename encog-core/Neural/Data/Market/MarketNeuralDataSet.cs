using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData.Temporal;
using Encog.Neural.NeuralData.Market.Loader;
using Encog.Util.Time;

namespace Encog.Neural.NeuralData.Market
{
    public class MarketNeuralDataSet : TemporalNeuralDataSet
    {
        /// <summary>
        /// The loader to use to obtain the data.
        /// </summary>
        private IMarketLoader loader;

        /// <summary>
        /// A map between the data points and actual data.
        /// </summary>
        private IDictionary<int, TemporalPoint> pointIndex =
            new Dictionary<int, TemporalPoint>();

        /// <summary>
        /// Construct a market data set object.
        /// </summary>
        /// <param name="loader">The loader to use to get the financial data.</param>
        /// <param name="inputWindowSize">The input window size, that is how many datapoints do we use to predict.</param>
        /// <param name="predictWindowSize">How many datapoints do we want to predict.</param>
        public MarketNeuralDataSet(IMarketLoader loader,
                 int inputWindowSize, int predictWindowSize)
            : base(inputWindowSize, predictWindowSize)
        {

            this.loader = loader;
            this.SequenceGrandularity = TimeUnit.DAYS;
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
            int sequence = GetSequenceFromDate(when);
            TemporalPoint result = this.pointIndex[sequence];

            if (result == null)
            {
                result = base.CreatePoint(when);
                this.pointIndex[result.Sequence] = result;
            }

            return result;
        }

        /// <summary>
        /// The loader that is being used for this set.
        /// </summary>
        public IMarketLoader Loader
        {
            get
            {
                return this.loader;
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
            if (this.StartingPoint == null)
            {
                this.StartingPoint = begin;
            }

            // clear out any loaded points
            this.Points.Clear();

            // first obtain a collection of symbols that need to be looked up
            IDictionary<TickerSymbol, object> set = new Dictionary<TickerSymbol, object>();
            foreach (TemporalDataDescription desc in this.Descriptions)
            {
                MarketDataDescription mdesc = (MarketDataDescription)desc;
                set[mdesc.Ticker] = null;
            }

            // now loop over each symbol and load the data
            foreach (TickerSymbol symbol in set.Keys)
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
            foreach (TemporalDataDescription desc in this.Descriptions)
            {
                MarketDataDescription mdesc = (MarketDataDescription)desc;

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
            ICollection<LoadedMarketData> data = this.Loader.Load(ticker,
                   null, from, to);
            foreach (LoadedMarketData item in data)
            {
                TemporalPoint point = this.CreatePoint(item.When);

                LoadPointFromMarketData(ticker, point, item);
            }
        }
    }
}
