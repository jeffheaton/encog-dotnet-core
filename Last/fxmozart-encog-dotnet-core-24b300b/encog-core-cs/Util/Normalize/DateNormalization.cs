using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil;

namespace Encog.Util.Normalize
{
    class DateNormalization
    {


        //we are going to try to normalize dates.

        ///
        /// Normalizes the day date.
        ///
        /// The adate.
        ///
        public static double[] NormalizeDayDate(DateTime adate)
        {
            double[] ReturnDay = new double[6];
            switch (adate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return ReturnDay = new double[6] { 0.7637, 0.4409, 0.3118, 0.2415, 0.1972, 0.1666 };
                    break;
                case DayOfWeek.Tuesday:
                    return ReturnDay = new double[6] { -0.7637, 0.4409, 0.3118, 0.2415, 0.1972, 0.1666 };
                    break;
                case DayOfWeek.Wednesday:
                    return ReturnDay = new double[6] { 0.0, -0.8819, 0.3118, 0.2415, 0.1972, 0.1666 };
                    break;
                case DayOfWeek.Thursday:
                    return ReturnDay = new double[6] { 0.0, 0.0, -0.9354, 0.2415, 0.1972, 0.1666 };
                    break;
                case DayOfWeek.Friday:
                    return ReturnDay = new double[6] { 0.0, 0.0, 0.0, -0.9660, 0.1972, 0.1666 };
                    break;
                case DayOfWeek.Saturday:
                    return ReturnDay = new double[6] { 0.0, 0.0, 0.0, 0.0, -0.9860, 0.1666 };
                    break;
                case DayOfWeek.Sunday:
                    return ReturnDay = new double[6] { 0.0, 0.0, 0.0, 0.0, 0.0, -1.0 };
                    break;
            }
            return ReturnDay;
        }


        /// <summary>
        /// Denormalize a double array into a day of week.
        /// </summary>
        /// <param name="adate">The adate.</param>
        /// <returns></returns>
        public static DayOfWeek DeNormalizeDayDate(double [] adate)
        {
            double[] ReturnDay = new double[6];
            int index = 0;
            foreach (double d in adate)
            {
                if (index == 0 && d == 0.7637)
                    return DayOfWeek.Monday;
                if (index == 0 && d == -0.7637)
                    return DayOfWeek.Tuesday;
                if (index == 2 && d == -0.8819)
                    return DayOfWeek.Wednesday;
                if (index == 3 && d == -0.9354)
                    return DayOfWeek.Thursday;
                if (index == 4 && d == -0.9660)
                    return DayOfWeek.Friday;
                if (index == 5 && d == -0.9860)
                    return DayOfWeek.Saturday;
                if (index == 6 && d == -1.0)
                    return DayOfWeek.Sunday;
                index++;
            }
            return 0;
        }



       
    }
}
