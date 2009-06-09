using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Encog.Util.MathUtil
{
    /// <summary>
    /// A simple class that prevents numbers from getting either too
    /// big or too small.
    /// </summary>
    public sealed class BoundNumbers
    {

        /// <summary>
        /// The logging object.
        /// </summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(BoundNumbers));

        /// <summary>
        /// Private constructor.
        /// </summary>
        private BoundNumbers()
        {

        }

        /// <summary>
        /// Too small of a number.
        /// </summary>
        public const double TOO_SMALL = -1.0E20;

        /// <summary>
        /// Too big of a number.
        /// </summary>
        public const double TOO_BIG = 1.0E20;

        /// <summary>
        /// Bound the number so that it does not become too big or too small.
        /// </summary>
        /// <param name="d">The number to check.</param>
        /// <returns>The new number. Only changed if it was too big or too small.</returns>
        public static double Bound(double d)
        {
            if (d < TOO_SMALL)
            {
                return TOO_SMALL;
            }
            else if (d > TOO_BIG)
            {
                return TOO_BIG;
            }
            else
            {
                return d;
            }
        }
    }

}
