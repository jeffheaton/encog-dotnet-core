using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data.Temporal;

namespace Encog.Neural.Data.Market
{
    public class MarketPoint : TemporalPoint
    {
        /// <summary>
        /// When to hold the data from.
        /// </summary>
        private DateTime when;


        /// <summary>
        /// Construct a MarketPoint with the specified date and size.
        /// </summary>
        /// <param name="when">When is this data from.</param>
        /// <param name="size">What is the size of the data.</param>
        public MarketPoint(DateTime when, int size) : base(size)
        {
            this.when = when;
        }

        public DateTime When
        {
            get
            {
                return when;
            }
        }
    }
}
