using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Time
{
    class DateUtil
    {
        /// <summary>
        /// January is 1.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime CreateDate(int month, int day, int year)
        {
            DateTime result = new DateTime(year, month, day); 
            return result;
        }
    }
}
