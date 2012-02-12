using System;


namespace Encog.Examples.RateSP500
{
    public class InterestRate : IComparable<InterestRate>
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
         * @return the effectiveDateInputDate
         */
        public DateTime getEffectiveDate()
        {
            return effectiveDate;
        }

        /**
         * @return the arate
         */
        public double getRate()
        {
            return rate;
        }

        /**
         * @param effectiveDateInputDate
         *            the effectiveDateInputDate to set
         */
        public void setEffectiveDate(DateTime effectiveDateInputDate)
        {
            effectiveDate = effectiveDateInputDate;
        }

        /**
         * @param arate
         *            the arate to set
         */
        public void setRate(double arate)
        {
            rate = arate;
        }
    }
}
