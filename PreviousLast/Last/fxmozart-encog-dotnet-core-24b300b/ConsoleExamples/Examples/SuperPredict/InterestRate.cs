using System;

namespace Encog.Examples.SuperPredict
{
    class InterestRate : IComparable<InterestRate>
    {
        private DateTime effectiveDate;
        private double rate;

        public InterestRate(DateTime effectiveDate, double rate)
        {
            this.effectiveDate = effectiveDate;
            this.rate = rate;
        }

        public int CompareTo(InterestRate other)
        {
            return getEffectiveDate().CompareTo(other.getEffectiveDate());
        }

        /**
         * @return the effectiveDate
         */
        public DateTime getEffectiveDate()
        {
            return this.effectiveDate;
        }

        /**
         * @return the rate
         */
        public double getRate()
        {
            return this.rate;
        }

        /**
         * @param effectiveDate
         *            the effectiveDate to set
         */
        public void setEffectiveDate(DateTime effectiveDate)
        {
            this.effectiveDate = effectiveDate;
        }

        /**
         * @param rate
         *            the rate to set
         */
        public void setRate(double rate)
        {
            this.rate = rate;
        }

    }
}