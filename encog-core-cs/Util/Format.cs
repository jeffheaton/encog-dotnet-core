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
        public const double HundredPercent = 100.0;

        /// <summary>
        /// Bytes in a KB.
        /// </summary>
        public const long MemoryK = 1024;

        /// <summary>
        /// Bytes in a MB.
        /// </summary>
        public const long MemoryMeg = (1024*MemoryK);

        /// <summary>
        /// Bytes in a GB.
        /// </summary>
        public const long MemoryGig = (1024*MemoryMeg);

        /// <summary>
        /// Bytes in a TB.
        /// </summary>
        public const long MemoryTera = (1024*MemoryGig);

        /// <summary>
        /// Seconds in a minute.
        /// </summary>
        public const int SecondsInaMinute = 60;

        /// <summary>
        /// Seconds in an hour.
        /// </summary>
        public const int SecondsInaHour = SecondsInaMinute*60;

        /// <summary>
        /// Seconds in a day.
        /// </summary>
        public const int SecondsInaDay = SecondsInaHour*24;

        /// <summary>
        /// Miliseconds in a day.
        /// </summary>
        public const long MiliInSec = 1000;

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
            if (Double.IsNaN(d) || Double.IsInfinity(d))
                return "NaN"; 
            return d.ToString("N" + i);
        }


        /// <summary>
        /// Format a memory amount, to something like 32 MB.
        /// </summary>
        /// <param name="memory">The amount of bytes.</param>
        /// <returns>The formatted memory size.</returns>
        public static String FormatMemory(long memory)
        {
            if (memory < MemoryK)
            {
                return memory + " bytes";
            }
            if (memory < MemoryMeg)
            {
                return FormatDouble((memory)/((double) MemoryK), 2) + " KB";
            }
            if (memory < MemoryGig)
            {
                return FormatDouble((memory)/((double) MemoryMeg), 2) + " MB";
            }
            if (memory < MemoryTera)
            {
                return FormatDouble((memory)/((double) MemoryGig), 2) + " GB";
            }
            return FormatDouble((memory)/((double) MemoryTera), 2) + " TB";
        }

        /// <summary>
        /// Format an integer.
        /// </summary>
        /// <param name="i">The integer.</param>
        /// <returns>The string.</returns>
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
            if( Double.IsNaN(e) || Double.IsInfinity(e) ) 
                return "NaN"; 
            return (e*100.0).ToString("N6") + "%";
        }

        /// <summary>
        /// Format a percent with no decimal places.
        /// </summary>
        /// <param name="e">The format to percent.</param>
        /// <returns>The formatted percent.</returns>
        public static String FormatPercentWhole(double e)
        {
            if (Double.IsNaN(e) || Double.IsInfinity(e))
                return "NaN"; 
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
            int days = seconds/SecondsInaDay;
            secondsCount -= days*SecondsInaDay;
            int hours = secondsCount/SecondsInaHour;
            secondsCount -= hours*SecondsInaHour;
            int minutes = secondsCount/SecondsInaMinute;
            secondsCount -= minutes*SecondsInaMinute;

            var result = new StringBuilder();

            if (days > 0)
            {
                result.Append(days);
                result.Append(days > 1 ? " days " : " day ");
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
            return p ? "Yes" : "No";
        }
    }
}
