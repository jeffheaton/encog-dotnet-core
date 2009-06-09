using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.MathUtil
{
    /// <summary>
    /// C# will sometimes return Math.NaN or Math.Infinity when numbers get to
    /// large or too small. This can have undesirable effects. This class provides
    /// some basic math functions that may be in danger of returning such a value.
    /// This class imposes a very large and small ceiling and floor to keep the
    /// numbers within range.
    /// </summary>
    public sealed class BoundMath
    {

        /// <summary>
        /// Private constructor.
        /// </summary>
        private BoundMath()
        {

        }

        /// <summary>
        /// Calculate the cos.
        /// </summary>
        /// <param name="a">The value passed to the function.</param>
        /// <returns>The result of the function.</returns>
        public static double Cos(double a)
        {
            return BoundNumbers.Bound(Math.Cos(a));
        }

        /// <summary>
        /// Calculate the exp.
        /// </summary>
        /// <param name="a">The value passed to the function.</param>
        /// <returns>The result of the function.</returns>
        public static double Exp(double a)
        {
            return BoundNumbers.Bound(Math.Exp(a));
        }

        /// <summary>
        /// Calculate the log.
        /// </summary>
        /// <param name="a">The value passed to the function.</param>
        /// <returns>The result of the function.</returns>
        public static double Log(double a)
        {
            return BoundNumbers.Bound(Math.Log(a));
        }

        /// <summary>
        /// Calculate the power of a number.
        /// </summary>
        /// <param name="a">The base.</param>
        /// <param name="b">The exponent.</param>
        /// <returns></returns>
        public static double Pow(double a, double b)
        {
            return BoundNumbers.Bound(Math.Pow(a, b));
        }

        /// <summary>
        /// Calculate the sin.
        /// </summary>
        /// <param name="a">The value passed to the function.</param>
        /// <returns>The result of the function.</returns>
        public static double Sin(double a)
        {
            return BoundNumbers.Bound(Math.Sin(a));
        }

        /// <summary>
        /// Calculate the square root.
        /// </summary>
        /// <param name="a">The value passed to the function.</param>
        /// <returns>The result of the function.</returns>
        public static double Sqrt(double a)
        {
            return Math.Sqrt(a);
        }
    }

}
