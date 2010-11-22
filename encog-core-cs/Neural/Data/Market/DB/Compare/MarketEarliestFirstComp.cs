using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Data.Market.DB.Compare
{
    class MarketEarliestFirstComp : IComparer<StoredData>
    {
        public int Compare(StoredData x, StoredData y)
        {
            // attempt to sort by date first
            int result = x.Date.CompareTo(y.Date);
            if (result != 0)
                return result;

            // if dates tie, then sort by time
            result = x.Time.CompareTo(y.Time);       
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
