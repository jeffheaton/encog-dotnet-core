// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

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
        /// Bytes in a KB.
        /// </summary>
        const long MEMORY_K = 1024;

        /// <summary>
        /// Bytes in a MB.
        /// </summary>
        const long MEMORY_MEG = (1024 * MEMORY_K);

        /// <summary>
        /// Bytes in a GB.
        /// </summary>
        const long MEMORY_GIG = (1024 * MEMORY_MEG);

        /// <summary>
        /// Bytes in a TB.
        /// </summary>
        const long MEMORY_TERA = (1024 * MEMORY_GIG);

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
        public static String FormatDouble(double d, int i)
        {
            return d.ToString("N" + i);
        }


        public static String FormatMemory(long memory)
        {
            if (memory < Format.MEMORY_K)
            {
                return memory + " bytes";
            }
            else if (memory < Format.MEMORY_MEG)
            {
                return FormatDouble( ((double)memory) / ((double)Format.MEMORY_K), 2)+" KB";
            }
            else if (memory < Format.MEMORY_GIG)
            {
                return FormatDouble(((double)memory) / ((double)Format.MEMORY_MEG), 2) + " MB";
            }
            else if (memory < Format.MEMORY_TERA)
            {
                return FormatDouble(((double)memory) / ((double)Format.MEMORY_GIG), 2) + " GB";
            }
            else 
            {
                return FormatDouble(((double)memory) / ((double)Format.MEMORY_TERA), 2) + " TB";
            }
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
            return String.Format("{0:n0}", i);
        }

        /// <summary>
        /// Format a percent.  Using 6 decimal places.
        /// </summary>
        /// <param name="e">The percent to format.</param>
        /// <returns>The formatted percent.</returns>
        public static String FormatPercent(double e)
        {
            return (e*100.0).ToString("N6") + "%";
        }

        /// <summary>
        /// Format a percent with no decimal places.
        /// </summary>
        /// <param name="e">The format to percent.</param>
        /// <returns>The formatted percent.</returns>
        public static String FormatPercentWhole(double e)
        {
            return (e*100.0).ToString("N0") + "%";
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
