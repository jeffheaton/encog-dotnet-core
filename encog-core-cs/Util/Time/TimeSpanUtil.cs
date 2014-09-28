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
    internal class TimeSpanUtil
    {
        private DateTime _from;
        private DateTime _to;

        public TimeSpanUtil(DateTime from, DateTime to)
        {
            _from = from;
            _to = to;
        }

        public DateTime From
        {
            get { return _from; }
        }

        public DateTime To
        {
            get { return _to; }
        }


        public long GetSpan(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Ticks:
                    return GetSpanTicks();
                case TimeUnit.Seconds:
                    return GetSpanSeconds();
                case TimeUnit.Minutes:
                    return GetSpanMinutes();
                case TimeUnit.Hours:
                    return GetSpanHours();
                case TimeUnit.Days:
                    return GetSpanDays();
                case TimeUnit.Weeks:
                    return GetSpanWeeks();
                case TimeUnit.Fortnights:
                    return GetSpanFortnights();
                case TimeUnit.Months:
                    return GetSpanMonths();
                case TimeUnit.Years:
                    return GetSpanYears();
                case TimeUnit.Scores:
                    return GetSpanScores();
                case TimeUnit.Centuries:
                    return GetSpanCenturies();
                case TimeUnit.Millennia:
                    return GetSpanMillennia();
                default:
                    return 0;
            }
        }

        private long GetSpanTicks()
        {
            TimeSpan span = _to.Subtract(_from);
            return span.Ticks;
        }

        private long GetSpanSeconds()
        {
            TimeSpan span = _to.Subtract(_from);
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
            return (_to.Month - _from.Month) + (_to.Year - _from.Year)*12;
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
