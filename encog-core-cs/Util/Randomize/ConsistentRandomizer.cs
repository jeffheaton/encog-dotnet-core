using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.MathUtil;

namespace Encog.Util.Randomize
{
    /// <summary>
    /// A randomizer that takes a seed and will always produce consistent results.
    /// </summary>
    public class ConsistentRandomizer : BasicRandomizer
    {

        /// <summary>
        /// The generator.
        /// </summary>
        private LinearCongruentialGenerator rand;


        /// <summary>
        /// The minimum value for the random range.
        /// </summary>
        private double Min { get; set; }

        /// <summary>
        /// The maximum value for the random range.
        /// </summary>
        private double Max { get; set; }

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        /// <param name="min">The minimum random value.</param>
        /// <param name="max">The maximum random value.</param>
        public ConsistentRandomizer(double min, double max)
        {
            this.Max = max;
            this.Min = min;
            this.rand = new LinearCongruentialGenerator(1000);
        }

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        /// <param name="seed">The seed for the random number generator.</param>
        /// <param name="min">The minimum random value.</param>
        /// <param name="max">The maximum random value.</param>
        public ConsistentRandomizer(int seed, double min,
                 double max)
        {
            this.rand = new LinearCongruentialGenerator(seed);
            this.Max = max;
            this.Min = min;
        }

        /// <summary>
        /// Generate a random number based on the range specified in the constructor.
        /// </summary>
        /// <param name="d">The range randomizer ignores this value.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return rand.Range(this.Min, this.Max);
        }

    }
}
