//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using System.Text;

namespace Encog.Util
{
    /// <summary>
    /// Provides the ability for Encog to format numbers and times.
    /// </summary>
    public class Format
    {
        /// <summary>
        /// One hundred percent.
        /// </summary>
        public const double HUNDRED_PERCENT = 100.0;

        /// <summary>
        /// Bytes in a KB.
        /// </summary>
        public const long MEMORY_K = 1024;

        /// <summary>
        /// Bytes in a MB.
        /// </summary>
        public const long MEMORY_MEG = (1024*MEMORY_K);

        /// <summary>
        /// Bytes in a GB.
        /// </summary>
        public const long MEMORY_GIG = (1024*MEMORY_MEG);

        /// <summary>
        /// Bytes in a TB.
        /// </summary>
        public const long MEMORY_TERA = (1024*MEMORY_GIG);

        /// <summary>
        /// Seconds in a minute.
        /// </summary>
        public const int SECONDS_INA_MINUTE = 60;

        /// <summary>
        /// Seconds in an hour.
        /// </summary>
        public const int SECONDS_INA_HOUR = SECONDS_INA_MINUTE*60;

        /// <summary>
        /// Seconds in a day.
        /// </summary>
        public const int SECONDS_INA_DAY = SECONDS_INA_HOUR*24;

        /// <summary>
        /// Miliseconds in a day.
        /// </summary>
        public const long MILI_IN_SEC = 1000;

        /// <summary>
        /// Private constructor.
        /// </summary>
        private Format()
        {
        }

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


        /// <summary>
        /// Format a memory amount, to something like 32 MB.
        /// </summary>
        /// <param name="memory">The amount of bytes.</param>
        /// <returns>The formatted memory size.</returns>
        public static String FormatMemory(long memory)
        {
            if (memory < MEMORY_K)
            {
                return memory + " bytes";
            }
            else if (memory < MEMORY_MEG)
            {
                return FormatDouble((memory)/((double) MEMORY_K), 2) + " KB";
            }
            else if (memory < MEMORY_GIG)
            {
                return FormatDouble((memory)/((double) MEMORY_MEG), 2) + " MB";
            }
            else if (memory < MEMORY_TERA)
            {
                return FormatDouble((memory)/((double) MEMORY_GIG), 2) + " GB";
            }
            else
            {
                return FormatDouble((memory)/((double) MEMORY_TERA), 2) + " TB";
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
            int days = seconds/SECONDS_INA_DAY;
            secondsCount -= days*SECONDS_INA_DAY;
            int hours = secondsCount/SECONDS_INA_HOUR;
            secondsCount -= hours*SECONDS_INA_HOUR;
            int minutes = secondsCount/SECONDS_INA_MINUTE;
            secondsCount -= minutes*SECONDS_INA_MINUTE;

            var result = new StringBuilder();

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
        /// Format a boolean to yes/no.
        /// </summary>
        /// <param name="p">The default answer.</param>
        /// <returns>A string form of the boolean.</returns>
        public static string FormatYesNo(bool p)
        {
            if (p)
                return "Yes";
            else
                return "No";
        }
    }
}
