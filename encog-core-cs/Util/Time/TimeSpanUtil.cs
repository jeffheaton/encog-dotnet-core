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

namespace Encog.Util.Time
{
    class TimeSpanUtil
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
            get
            {
                return this.from;
            }
        }

        public DateTime To
        {
            get
            {
                return this.to;
            }
        }


        public long GetSpan(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.SECONDS: return GetSpanSeconds();
                case TimeUnit.MINUTES: return GetSpanMinutes();
                case TimeUnit.HOURS: return GetSpanHours();
                case TimeUnit.DAYS: return GetSpanDays();
                case TimeUnit.WEEKS: return GetSpanWeeks();
                case TimeUnit.FORTNIGHTS: return GetSpanFortnights();
                case TimeUnit.MONTHS: return GetSpanMonths();
                case TimeUnit.YEARS: return GetSpanYears();
                case TimeUnit.SCORES: return GetSpanScores();
                case TimeUnit.CENTURIES: return GetSpanCenturies();
                case TimeUnit.MILLENNIA: return GetSpanMillennia();
                default: return 0;
            }

        }

        private long GetSpanSeconds()
        {
            TimeSpan span = this.to.Subtract(this.from);
            return span.Ticks/TimeSpan.TicksPerSecond;
        }

        private long GetSpanMinutes()
        {
            return GetSpanSeconds() / 60;
        }

        private long GetSpanHours()
        {
            return GetSpanMinutes() / 60;
        }

        private long GetSpanDays()
        {
            return GetSpanHours() / 24;
        }

        private long GetSpanWeeks()
        {
            return GetSpanDays() / 7;
        }

        private long GetSpanFortnights()
        {
            return GetSpanWeeks() / 2;
        }

        private long GetSpanMonths()
        {
            return (to.Month - from.Month) + (to.Year - from.Year) * 12;
        }

        private long GetSpanYears()
        {
            return GetSpanMonths() / 12;
        }

        private long GetSpanScores()
        {
            return GetSpanYears() / 20;
        }

        private long GetSpanCenturies()
        {
            return GetSpanYears() / 100;
        }

        private long GetSpanMillennia()
        {
            return GetSpanYears() / 1000;
        }
    }
}
