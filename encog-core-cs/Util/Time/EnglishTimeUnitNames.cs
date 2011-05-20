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
