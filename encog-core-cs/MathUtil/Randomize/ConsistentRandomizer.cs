using Encog.Neural.Networks;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// A randomizer that takes a seed and will always produce consistent results.
    /// </summary>
    ///
    public class ConsistentRandomizer : BasicRandomizer
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
        /// The generator.
        /// </summary>
        ///
        private readonly LinearCongruentialGenerator rand;

        /// <summary>
        /// The seed.
        /// </summary>
        ///
        private readonly int seed;

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        ///
        /// <param name="min_0">The minimum random value.</param>
        /// <param name="max_1">The maximum random value.</param>
        public ConsistentRandomizer(double min_0, double max_1) : this(min_0,max_1, 1000)
        {
        }

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        ///
        /// <param name="min_0">The minimum random value.</param>
        /// <param name="max_1">The maximum random value.</param>
        /// <param name="seed_2">The seed value.</param>
        public ConsistentRandomizer(double min_0, double max_1,
                                    int seed_2)
        {
            max = max_1;
            min = min_0;
            seed = seed_2;
            rand = new LinearCongruentialGenerator(seed_2);
        }

        /// <summary>
        /// Generate a random number based on the range specified in the constructor.
        /// </summary>
        ///
        /// <param name="d">The range randomizer ignores this value.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return rand.Range(min, max);
        }

        /// <summary>
        /// Randomize the network.
        /// </summary>
        ///
        /// <param name="network">The network to randomize.</param>
        public void Randomize(BasicNetwork network)
        {
            rand.Seed = seed;
            base.Randomize(network);
        }
    }
}