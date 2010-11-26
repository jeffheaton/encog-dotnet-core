using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Loader.CSV
{
    public enum CSVDataItem
    {
        Ticker,
        Ignore,
        Open,
        Close,
        High,
        Low,
        Volume,
        DateAndTime,
        DateOnly,
        TimeOnly
    }
}
