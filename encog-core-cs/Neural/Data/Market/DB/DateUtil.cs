using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.NeuralData.Market.DB
{
    public static class DateUtil
    {
        public const uint YEAR_OFFSET = 10000;
        public const uint MONTH_OFFSET = 100;

        public static ulong DateTime2Long(DateTime time)
        {
            return (ulong)(time.Day + (time.Month * MONTH_OFFSET) + (time.Year * YEAR_OFFSET));
        }

        public static DateTime Long2DateTime(ulong l)
        {
            long rest = (long)l;
            int year = (int)(rest / YEAR_OFFSET);
            rest-=year*YEAR_OFFSET;
            int month = (int)(rest / MONTH_OFFSET);
            rest -= month * MONTH_OFFSET;
            int day = (int)rest;
            return new DateTime(year, month, day);
        }

        public static DateTime StripTime(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }

        public static bool HaveSameDate(DateTime d1, DateTime d2)
        {
            return ((d1.Day == d2.Day) && (d1.Month == d2.Month) && (d1.Year == d2.Year));
        }
    }
}
