using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.MathUtil
{
    /// <summary>
    /// Math functions used by Encog.
    /// </summary>
    public class EncogMath
    {
        /// <summary>
        /// Calculate sqrt(a^2 + b^2) without under/overflow.
        /// </summary>
        /// <param name="a">The a value.</param>
        /// <param name="b">The b value.</param>
        /// <returns>The result.</returns>
        public static double Hypot(double a, double b)
        {
            double r;
            if (Math.Abs(a) > Math.Abs(b))
            {
                r = b / a;
                r = Math.Abs(a) * Math.Sqrt(1 + r * r);
            }
            else if (b != 0)
            {
                r = a / b;
                r = Math.Abs(b) * Math.Sqrt(1 + r * r);
            }
            else
            {
                r = 0.0;
            }
            return r;
        }


        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        /// <param name="deg">Degrees</param>
        /// <returns>Radians</returns>
        public static double Deg2rad(double deg)
        {
            return deg * (Math.PI / 180.0);
        }

        

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        /// <param name="rad">Radians.</param>
        /// <returns>Degrees.</returns>
        public static double Rad2deg(double rad)
        {
            return rad * (180.0 / Math.PI);
        }
    }
}
