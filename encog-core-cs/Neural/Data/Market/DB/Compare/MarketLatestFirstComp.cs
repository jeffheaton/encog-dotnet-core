using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Data.Market.DB.Compare
{
    public class MarketLatestFirstComp : IComparer<StoredData>
    {
        private static int Inverse(int cmp)
        {
            if (cmp < 0)
            {
                return 1;
            }
            else if (cmp > 0)
            {
                return -1;
            }

            return 0;
        }

        public int Compare(StoredData x, StoredData y)
        {
            // attempt to sort by date first
            int result = MarketLatestFirstComp.Inverse( x.Date.CompareTo(y.Date) );
            if (result != 0)
                return result;

            // if dates tie, then sort by time
            result = MarketLatestFirstComp.Inverse( x.Time.CompareTo(y.Time) );
            if (result != 0)
                return result;

            // if times tie, adjustment data always goes first
            if (x is StoredAdjustmentData)
            {
                return -1;
            }
            else
            {
                return 1;
            }

        }

    }
}
