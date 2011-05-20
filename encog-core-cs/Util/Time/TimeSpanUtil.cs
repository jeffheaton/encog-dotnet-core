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
    internal class TimeSpanUtil
    {
        private DateTime from;
        private DateTime to;

        public TimeSpanUtil(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        public DateTime From
        {
            get { return from; }
        }

        public DateTime To
        {
            get { return to; }
        }


        public long GetSpan(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.SECONDS:
                    return GetSpanSeconds();
                case TimeUnit.MINUTES:
                    return GetSpanMinutes();
                case TimeUnit.HOURS:
                    return GetSpanHours();
                case TimeUnit.DAYS:
                    return GetSpanDays();
                case TimeUnit.WEEKS:
                    return GetSpanWeeks();
                case TimeUnit.FORTNIGHTS:
                    return GetSpanFortnights();
                case TimeUnit.MONTHS:
                    return GetSpanMonths();
                case TimeUnit.YEARS:
                    return GetSpanYears();
                case TimeUnit.SCORES:
                    return GetSpanScores();
                case TimeUnit.CENTURIES:
                    return GetSpanCenturies();
                case TimeUnit.MILLENNIA:
                    return GetSpanMillennia();
                default:
                    return 0;
            }
        }

        private long GetSpanSeconds()
        {
            TimeSpan span = to.Subtract(from);
            return span.Ticks/TimeSpan.TicksPerSecond;
        }

        private long GetSpanMinutes()
        {
            return GetSpanSeconds()/60;
        }

        private long GetSpanHours()
        {
            return GetSpanMinutes()/60;
        }

        private long GetSpanDays()
        {
            return GetSpanHours()/24;
        }

        private long GetSpanWeeks()
        {
            return GetSpanDays()/7;
        }

        private long GetSpanFortnights()
        {
            return GetSpanWeeks()/2;
        }

        private long GetSpanMonths()
        {
            return (to.Month - from.Month) + (to.Year - from.Year)*12;
        }

        private long GetSpanYears()
        {
            return GetSpanMonths()/12;
        }

        private long GetSpanScores()
        {
            return GetSpanYears()/20;
        }

        private long GetSpanCenturies()
        {
            return GetSpanYears()/100;
        }

        private long GetSpanMillennia()
        {
            return GetSpanYears()/1000;
        }
    }
}
