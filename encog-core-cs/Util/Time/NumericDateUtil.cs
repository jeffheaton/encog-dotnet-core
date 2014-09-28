//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
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
        public const uint YearOffset = 10000;

        /// <summary>
        /// The numeric offset for a month.
        /// </summary>
        public const uint MonthOffset = 100;

        /// <summary>
        /// The numeric offset for an hour.
        /// </summary>
        public const uint HourOffset = 10000;

        /// <summary>
        /// The numeric offset for a minute.
        /// </summary>
        public const uint MinuteOffset = 100;

        /// <summary>
        /// Convert a date/time to a long.
        /// </summary>
        /// <param name="time">The time to convert.</param>
        /// <returns>A numeric date.</returns>
        public static ulong DateTime2Long(DateTime time)
        {
            return (ulong) (time.Day + (time.Month*MonthOffset) + (time.Year*YearOffset));
        }

        /// <summary>
        /// Convert a numeric date time to a regular date time.
        /// </summary>
        /// <param name="l">The numeric date time.</param>
        /// <returns>The converted date/time.</returns>
        public static DateTime Long2DateTime(ulong l)
        {
            var rest = (long) l;
            var year = (int) (rest/YearOffset);
            rest -= year*YearOffset;
            var month = (int) (rest/MonthOffset);
            rest -= month*MonthOffset;
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
            var hour = (int) (rest/HourOffset);
            rest -= (uint) (hour*HourOffset);
            var minute = (int) (rest/MonthOffset);
            rest -= (uint) (minute*MinuteOffset);
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
            return (uint) (time.Second + (time.Minute*MinuteOffset) + (time.Hour*HourOffset));
        }

        /// <summary>
        /// Get the year part of a numeric date.
        /// </summary>
        /// <param name="date">The numeric date.</param>
        /// <returns>The year.</returns>
        public static int GetYear(ulong date)
        {
            return (int) (date/YearOffset);
        }

        /// <summary>
        /// Get the year month of a numeric date.
        /// </summary>
        /// <param name="l">The numeric date.</param>
        /// <returns>The month.</returns>
        public static int GetMonth(ulong l)
        {
            var rest = (long) l;
            var year = (int) (rest/YearOffset);
            rest -= year*YearOffset;
            return (int) (rest/MonthOffset);
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
            var hour = (int) (rest/HourOffset);
            rest -= (uint) (hour*HourOffset);
            var minute = (int) (rest/MonthOffset);

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
