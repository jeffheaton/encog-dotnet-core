using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data.Temporal;

namespace Encog.ML.Data.Market.FinanceDataSet
{
    class FinancePoints : TemporalPoint
    {
         /// <summary>
        /// When to hold the data from.
        /// </summary>
        private readonly DateTime _when;


        /// <summary>
        /// Construct a MarketPoint with the specified date and size.
        /// </summary>
        /// <param name="when">When is this data from.</param>
        /// <param name="size">What is the size of the data.</param>
        public FinancePoints(DateTime when, int size)
            : base(size)
        {
            _when = when;
        }

        /// <summary>
        /// When is this point from.
        /// </summary>
        public DateTime When
        {
            get { return _when; }
        }
    }
}
