using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.MathUtil
{
    public class IntRange
    {
        /**
 * The low end of the range.
 */
        public int High { get; set; }

        /**
         * The high end of the range.
         */
        public int Low { get; set; }


        /**
         * Construct an integer range.
         * @param high The high  end of the range.
         * @param low The low end of the range.
         */
        public IntRange(int high, int low)
        {
            this.High = high;
            this.Low = low;
        }
    }
}
