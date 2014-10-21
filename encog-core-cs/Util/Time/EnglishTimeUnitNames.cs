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
    internal class EnglishTimeUnitNames : ITimeUnitNames
    {
        #region ITimeUnitNames Members

        public String Code(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return "sec";
                case TimeUnit.Minutes:
                    return "min";
                case TimeUnit.Hours:
                    return "hr";
                case TimeUnit.Days:
                    return "d";
                case TimeUnit.Weeks:
                    return "w";
                case TimeUnit.Fortnights:
                    return "fn";
                case TimeUnit.Months:
                    return "m";
                case TimeUnit.Years:
                    return "y";
                case TimeUnit.Decades:
                    return "dec";
                case TimeUnit.Scores:
                    return "sc";
                case TimeUnit.Centuries:
                    return "c";
                case TimeUnit.Millennia:
                    return "m";
                default:
                    return "unk";
            }
        }

        public String Plural(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return "seconds";
                case TimeUnit.Minutes:
                    return "minutes";
                case TimeUnit.Hours:
                    return "hours";
                case TimeUnit.Days:
                    return "days";
                case TimeUnit.Weeks:
                    return "weeks";
                case TimeUnit.Fortnights:
                    return "fortnights";
                case TimeUnit.Months:
                    return "months";
                case TimeUnit.Years:
                    return "years";
                case TimeUnit.Decades:
                    return "decades";
                case TimeUnit.Scores:
                    return "scores";
                case TimeUnit.Centuries:
                    return "centures";
                case TimeUnit.Millennia:
                    return "millennia";
                default:
                    return "unknowns";
            }
        }

        public String Singular(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return "second";
                case TimeUnit.Minutes:
                    return "minute";
                case TimeUnit.Hours:
                    return "hour";
                case TimeUnit.Days:
                    return "day";
                case TimeUnit.Weeks:
                    return "week";
                case TimeUnit.Fortnights:
                    return "fortnight";
                case TimeUnit.Months:
                    return "month";
                case TimeUnit.Years:
                    return "year";
                case TimeUnit.Decades:
                    return "decade";
                case TimeUnit.Scores:
                    return "score";
                case TimeUnit.Centuries:
                    return "century";
                case TimeUnit.Millennia:
                    return "millenium";
                default:
                    return "unknown";
            }
        }

        #endregion
    }
}
