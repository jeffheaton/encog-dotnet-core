using System;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// A randomizer that will create random weight and bias values that are between
    /// a specified range.
    /// </summary>
    ///
    public class RangeRandomizer : BasicRandomizer
    {
        /// <summary>
        /// The maximum value for the random range.
        /// </summary>
        ///
        private readonly double max;

        /// <summary>
        /// The minimum value for the random range.
        /// </summary>
        ///
        private readonly double min;

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        ///
        /// <param name="min_0">The minimum random value.</param>
        /// <param name="max_1">The maximum random value.</param>
        public RangeRandomizer(double min_0, double max_1)
        {
            max = max_1;
            min = min_0;
        }


        /// <value>the min</value>
        public double Min
        {
            /// <returns>the min</returns>
            get { return min; }
        }


        /// <value>the max</value>
        public double Max
        {
            /// <returns>the max</returns>
            get { return max; }
        }

        public static int RandomInt(int min, int max)
        {
            return (int) Randomize(min, max + 1);
        }

        /// <summary>
        /// Generate a random number in the specified range.
        /// </summary>
        ///
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A random number.</returns>
        public static double Randomize(double min, double max)
        {
            double range = max - min;
            return (range*(new Random()).Next()) + min;
        }

        /// <summary>
        /// Generate a random number based on the range specified in the constructor.
        /// </summary>
        ///
        /// <param name="d">The range randomizer ignores this value.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return NextDouble(min, max);
        }
    }
}