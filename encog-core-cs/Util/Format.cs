using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util
{
    /// <summary>
    /// Provides the ability for Encog to format numbers and times.
    /// </summary>
    public class Format
    {
        /// <summary>
        /// Seconds in a minute.
        /// </summary>
        public const int SECONDS_INA_MINUTE = 60;

        /// <summary>
        /// Seconds in an hour.
        /// </summary>
        public const int SECONDS_INA_HOUR = Format.SECONDS_INA_MINUTE * 60;

        /// <summary>
        /// Seconds in a day.
        /// </summary>
        public const int SECONDS_INA_DAY = Format.SECONDS_INA_HOUR * 24;

        /// <summary>
        /// Format a double.
        /// </summary>
        /// <param name="d">The double value to format.</param>
        /// <param name="i">The number of decimal places.</param>
        /// <returns>The double as a string.</returns>
        public static Object FormatDouble(double d, int i)
        {
            return d.ToString("N" + i);
        }

        /**
         * Format an integer.
         * 
         * @param i
         *            The integer to format.
         * @return The integer as a string.
         */
        public static String FormatInteger(int i)
        {
            return String.Format("{0:n}", i);
        }

        /// <summary>
        /// Format a percent.  Using 6 decimal places.
        /// </summary>
        /// <param name="e">The percent to format.</param>
        /// <returns>The formatted percent.</returns>
        public static String FormatPercent(double e)
        {
            return "%" + e.ToString("N6");
        }

        /// <summary>
        /// Format a percent with no decimal places.
        /// </summary>
        /// <param name="e">The format to percent.</param>
        /// <returns>The formatted percent.</returns>
        public static String FormatPercentWhole(double e)
        {
            return "%" + e.ToString("N0");
        }

        /// <summary>
        /// Format a time span as seconds, minutes, hours and days.
        /// </summary>
        /// <param name="seconds">The number of seconds in the timespan.</param>
        /// <returns>The formatted timespan.</returns>
        public static String FormatTimeSpan(int seconds)
        {
            int secondsCount = seconds;
            int days = seconds / Format.SECONDS_INA_DAY;
            secondsCount -= days * Format.SECONDS_INA_DAY;
            int hours = secondsCount / Format.SECONDS_INA_HOUR;
            secondsCount -= hours * Format.SECONDS_INA_HOUR;
            int minutes = secondsCount / Format.SECONDS_INA_MINUTE;
            secondsCount -= minutes * Format.SECONDS_INA_MINUTE;

            StringBuilder result = new StringBuilder();

            if (days > 0)
            {
                result.Append(days);
                if (days > 1)
                {
                    result.Append(" days ");
                }
                else
                {
                    result.Append(" day ");
                }
            }

            result.Append(hours.ToString("00"));
            result.Append(':');
            result.Append(minutes.ToString("00"));
            result.Append(':');
            result.Append(secondsCount.ToString("00"));

            return result.ToString();
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        private Format()
        {
        }
    }
}
