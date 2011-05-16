namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// A randomizer that will create always set the random number to a const value,
    /// used mainly for testing.
    /// </summary>
    ///
    public class ConstRandomizer : BasicRandomizer
    {
        /// <summary>
        /// The constant value.
        /// </summary>
        ///
        private readonly double value_ren;

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        ///
        /// <param name="value">The constant value.</param>
        public ConstRandomizer(double value_ren)
        {
            this.value_ren = value_ren;
        }

        /// <summary>
        /// Generate a random number based on the range specified in the constructor.
        /// </summary>
        ///
        /// <param name="d">The range randomizer ignores this value.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return value_ren;
        }
    }
}