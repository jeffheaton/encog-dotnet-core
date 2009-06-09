using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Encog.Util.Randomize
{
    /// <summary>
    /// A randomizer that distorts what is already present in the neural network.
    /// </summary>
    public class Distort : BasicRandomizer
    {

        /// <summary>
        /// The factor to use to distort the numbers.
        /// </summary>
        private double factor;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(Distort));

        /// <summary>
        /// Construct a distort randomizer for the specified factor.
        /// </summary>
        /// <param name="factor">The randomizer factor.</param>
        public Distort(double factor)
        {
            this.factor = factor;
        }

        /// <summary>
        /// Distort the random number by the factor that was specified 
        /// in the constructor.
        /// </summary>
        /// <param name="d">The number to distort.</param>
        /// <returns>The result.</returns>
        public override double Randomize(double d)
        {
            Random rand = new Random();
            return d + (this.factor - (rand.NextDouble() * this.factor * 2));
        }

    }

}
