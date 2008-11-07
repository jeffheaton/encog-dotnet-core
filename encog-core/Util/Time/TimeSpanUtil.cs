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
            return span.Seconds;
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
