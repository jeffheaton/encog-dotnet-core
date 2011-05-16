// Encog(tm) Artificial Intelligence Framework v2.5
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

namespace Encog.Util.Time
{
    internal class EnglishTimeUnitNames : ITimeUnitNames
    {
        #region ITimeUnitNames Members

        public String Code(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.SECONDS:
                    return "sec";
                case TimeUnit.MINUTES:
                    return "min";
                case TimeUnit.HOURS:
                    return "hr";
                case TimeUnit.DAYS:
                    return "d";
                case TimeUnit.WEEKS:
                    return "w";
                case TimeUnit.FORTNIGHTS:
                    return "fn";
                case TimeUnit.MONTHS:
                    return "m";
                case TimeUnit.YEARS:
                    return "y";
                case TimeUnit.DECADES:
                    return "dec";
                case TimeUnit.SCORES:
                    return "sc";
                case TimeUnit.CENTURIES:
                    return "c";
                case TimeUnit.MILLENNIA:
                    return "m";
                default:
                    return "unk";
            }
        }

        public String Plural(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.SECONDS:
                    return "seconds";
                case TimeUnit.MINUTES:
                    return "minutes";
                case TimeUnit.HOURS:
                    return "hours";
                case TimeUnit.DAYS:
                    return "days";
                case TimeUnit.WEEKS:
                    return "weeks";
                case TimeUnit.FORTNIGHTS:
                    return "fortnights";
                case TimeUnit.MONTHS:
                    return "months";
                case TimeUnit.YEARS:
                    return "years";
                case TimeUnit.DECADES:
                    return "decades";
                case TimeUnit.SCORES:
                    return "scores";
                case TimeUnit.CENTURIES:
                    return "centures";
                case TimeUnit.MILLENNIA:
                    return "millennia";
                default:
                    return "unknowns";
            }
        }

        public String Singular(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.SECONDS:
                    return "second";
                case TimeUnit.MINUTES:
                    return "minute";
                case TimeUnit.HOURS:
                    return "hour";
                case TimeUnit.DAYS:
                    return "day";
                case TimeUnit.WEEKS:
                    return "week";
                case TimeUnit.FORTNIGHTS:
                    return "fortnight";
                case TimeUnit.MONTHS:
                    return "month";
                case TimeUnit.YEARS:
                    return "year";
                case TimeUnit.DECADES:
                    return "decade";
                case TimeUnit.SCORES:
                    return "score";
                case TimeUnit.CENTURIES:
                    return "century";
                case TimeUnit.MILLENNIA:
                    return "millenium";
                default:
                    return "unknown";
            }
        }

        #endregion
    }
}