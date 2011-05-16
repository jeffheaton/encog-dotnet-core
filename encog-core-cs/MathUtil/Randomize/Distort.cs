namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// A randomizer that distorts what is already present in the neural network.
    /// </summary>
    ///
    public class Distort : BasicRandomizer
    {
        /// <summary>
        /// The factor to use to distort the numbers.
        /// </summary>
        ///
        private readonly double factor;

        /// <summary>
        /// Construct a distort randomizer for the specified factor.
        /// </summary>
        ///
        /// <param name="factor_0">The randomizer factor.</param>
        public Distort(double factor_0)
        {
            factor = factor_0;
        }

        /// <summary>
        /// Distort the random number by the factor that was specified in the
        /// constructor.
        /// </summary>
        ///
        /// <param name="d">The number to distort.</param>
        /// <returns>The result.</returns>
        public override double Randomize(double d)
        {
            return d + (factor - (NextDouble()*factor*2));
        }
    }
}