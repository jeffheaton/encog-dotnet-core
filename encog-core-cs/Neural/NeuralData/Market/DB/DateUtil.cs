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

        public static int GetYear(ulong date)
        {
            return (int) (date / YEAR_OFFSET);
        }

        public static int GetMonth(ulong l)
        {
            long rest = (long)l;
            int year = (int)(rest / YEAR_OFFSET);
            rest -= year * YEAR_OFFSET;
            return (int)(rest / MONTH_OFFSET);
        }

        public static int GetMinutePeriod(uint time, int period)
        {
            uint rest = (uint)time;
            int hour = (int)(rest / HOUR_OFFSET);
            rest -= (uint)(hour * HOUR_OFFSET);
            int minute = (int)(rest / MONTH_OFFSET);

            int minutes = minute + (hour * 60);
            return minutes / period;

        }

        public static int GetDayOfWeek(ulong p)
        {
            DateTime t = Long2DateTime(p);
            switch (t.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 0;
                case DayOfWeek.Monday:
                    return 1;
                case DayOfWeek.Tuesday:
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                default:
                    // no way this should happen!
                    return -1;
            }
        }
    }
}
