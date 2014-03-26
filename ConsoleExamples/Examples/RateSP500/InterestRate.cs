//
// Encog(tm) Core v3.2 - .Net Version
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
