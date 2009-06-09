using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Encog.Util.Randomize
{
    /// <summary>
    /// A randomizer that will create random weight and threshold values that are
    /// between a specified range.
    /// </summary>
    public class RangeRandomizer : BasicRandomizer
    {
        /// <summary>
        /// Generate a random number in the specified range.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A random number.</returns>
        public static double Randomize(double min, double max)
        {
            double range = max - min;
            Random rand = new Random();
            return (range * rand.NextDouble()) + min;
        }

        /// <summary>
        /// The minimum value for the random range.
        /// </summary>
        private double min;

        /// <summary>
        /// The maximum value for the random range.
        /// </summary>
        private double max;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(RangeRandomizer));

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        /// <param name="min">The minimum random value.</param>
        /// <param name="max">The maximum random value.</param>
        public RangeRandomizer(double min, double max)
        {
            this.max = max;
            this.min = min;
        }

        /// <summary>
        /// Generate a random number based on the range specified in the constructor.
        /// </summary>
        /// <param name="d">The range randomizer ignores this value.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return RangeRandomizer.Randomize(this.min, this.max);
        }

    }

}
