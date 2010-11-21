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

        public const uint HOUR_OFFSET = 10000;
        public const uint MINUTE_OFFSET = 100;

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

        internal static DateTime Int2Time(DateTime date, uint i)
        {
            uint rest = (uint)i;
            int hour = (int)(rest / HOUR_OFFSET);
            rest -= (uint)(hour * HOUR_OFFSET);
            int minute = (int)(rest / MONTH_OFFSET);
            rest -= (uint)(minute * MINUTE_OFFSET);
            int second = (int)rest;            
            return new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
        }

        internal static uint Time2Int(DateTime time)
        {
            return (uint)(time.Second + (time.Minute * MINUTE_OFFSET) + (time.Hour * HOUR_OFFSET));
        }
    }
}
