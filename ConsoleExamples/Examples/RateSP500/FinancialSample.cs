using System;
using System.Text;
using Encog.Util.CSV;

namespace Encog.Examples.RateSP500
{
    public  class FinancialSample : IComparable<FinancialSample>
    {
        private double amount;
        private double rate;
        private DateTime date;
        private double percent;

        public int CompareTo(FinancialSample other)
        {
            return getDate().CompareTo(other.getDate());
        }

        /**
         * @return the amount
         */
        public double getAmount()
        {
            return this.amount;
        }

        /**
         * @return the date
         */
        public DateTime getDate()
        {
            return this.date;
        }

        /**
         * @return the percent
         */
        public double getPercent()
        {
            return this.percent;
        }

        /**
         * @return the rate
         */
        public double getRate()
        {
            return this.rate;
        }

        /**
         * @param amount
         *            the amount to set
         */
        public void setAmount(double amount)
        {
            this.amount = amount;
        }

        /**
         * @param date
         *            the date to set
         */
        public void setDate(DateTime date)
        {
            this.date = date;
        }

        /**
         * @param percent
         *            the percent to set
         */
        public void setPercent(double percent)
        {
            this.percent = percent;
        }

        /**
         * @param rate
         *            the rate to set
         */
        public void setRate(double rate)
        {
            this.rate = rate;
        }

        override public String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(ReadCSV.ParseDate(date.ToShortDateString(),"yyyy-MM-dd"));
            result.Append(", Amount: ");
            result.Append(this.amount);
            result.Append(", Prime Rate: ");
            result.Append(this.rate);
            result.Append(", Percent from Previous: ");
            result.Append(this.percent.ToString("N2"));
            return result.ToString();
        }

    }
}
