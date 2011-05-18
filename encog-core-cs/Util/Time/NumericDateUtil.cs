using System;

namespace Encog.Util.Time
{
    /// <summary>
    /// A utility for storing dates as numeric values.
    /// </summary>
    public static class NumericDateUtil
    {
        /// <summary>
        /// The numeric offset for a year.
        /// </summary>
        public const uint YEAR_OFFSET = 10000;

        /// <summary>
        /// The numeric offset for a month.
        /// </summary>
        public const uint MONTH_OFFSET = 100;

        /// <summary>
        /// The numeric offset for an hour.
        /// </summary>
        public const uint HOUR_OFFSET = 10000;

        /// <summary>
        /// The numeric offset for a minute.
        /// </summary>
        public const uint MINUTE_OFFSET = 100;

        /// <summary>
        /// Convert a date/time to a long.
        /// </summary>
        /// <param name="time">The time to convert.</param>
        /// <returns>A numeric date.</returns>
        public static ulong DateTime2Long(DateTime time)
        {
            return (ulong) (time.Day + (time.Month*MONTH_OFFSET) + (time.Year*YEAR_OFFSET));
        }

        /// <summary>
        /// Convert a numeric date time to a regular date time.
        /// </summary>
        /// <param name="l">The numeric date time.</param>
        /// <returns>The converted date/time.</returns>
        public static DateTime Long2DateTime(ulong l)
        {
            var rest = (long) l;
            var year = (int) (rest/YEAR_OFFSET);
            rest -= year*YEAR_OFFSET;
            var month = (int) (rest/MONTH_OFFSET);
            rest -= month*MONTH_OFFSET;
            var day = (int) rest;
            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Strip the time element.
        /// </summary>
        /// <param name="dt">The time-date element to strip.</param>
        /// <returns>A new date-time with the time stripped.</returns>
        public static DateTime StripTime(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }

        /// <summary>
        /// Determine of two values have the same date.
        /// </summary>
        /// <param name="d1">The first date/time.</param>
        /// <param name="d2">The second date/time.</param>
        /// <returns>True, if they have the same date.</returns>
        public static bool HaveSameDate(DateTime d1, DateTime d2)
        {
            return ((d1.Day == d2.Day) && (d1.Month == d2.Month) && (d1.Year == d2.Year));
        }

        /// <summary>
        /// Convert an int to a time.
        /// </summary>
        /// <param name="date">The date-time that provides date information.</param>
        /// <param name="i">The int that holds the time.</param>
        /// <returns>The converted date/time.</returns>
        internal static DateTime Int2Time(DateTime date, uint i)
        {
            uint rest = i;
            var hour = (int) (rest/HOUR_OFFSET);
            rest -= (uint) (hour*HOUR_OFFSET);
            var minute = (int) (rest/MONTH_OFFSET);
            rest -= (uint) (minute*MINUTE_OFFSET);
            var second = (int) rest;
            return new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
        }

        /// <summary>
        /// Convert a time to an int.
        /// </summary>
        /// <param name="time">The time to convert.</param>
        /// <returns>The time as an int.</returns>
        internal static uint Time2Int(DateTime time)
        {
            return (uint) (time.Second + (time.Minute*MINUTE_OFFSET) + (time.Hour*HOUR_OFFSET));
        }

        /// <summary>
        /// Get the year part of a numeric date.
        /// </summary>
        /// <param name="date">The numeric date.</param>
        /// <returns>The year.</returns>
        public static int GetYear(ulong date)
        {
            return (int) (date/YEAR_OFFSET);
        }

        /// <summary>
        /// Get the year month of a numeric date.
        /// </summary>
        /// <param name="l">The numeric date.</param>
        /// <returns>The month.</returns>
        public static int GetMonth(ulong l)
        {
            var rest = (long) l;
            var year = (int) (rest/YEAR_OFFSET);
            rest -= year*YEAR_OFFSET;
            return (int) (rest/MONTH_OFFSET);
        }

        /// <summary>
        /// Get the minute period.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="period">The period size, in minutes.</param>
        /// <returns>The number of minutes per period.</returns>
        public static int GetMinutePeriod(uint time, int period)
        {
            uint rest = time;
            var hour = (int) (rest/HOUR_OFFSET);
            rest -= (uint) (hour*HOUR_OFFSET);
            var minute = (int) (rest/MONTH_OFFSET);

            int minutes = minute + (hour*60);
            return minutes/period;
        }

        /// <summary>
        /// Combine a date and a time.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="time">The time.</param>
        /// <returns>The combined time.</returns>
        public static ulong Combine(ulong date, uint time)
        {
            return (date*1000000) + time;
        }

        /// <summary>
        /// Get the day of the week for the specified numeric date.
        /// </summary>
        /// <param name="p">The time to check.</param>
        /// <returns>The day of the week, 0 is a sunday.</returns>
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