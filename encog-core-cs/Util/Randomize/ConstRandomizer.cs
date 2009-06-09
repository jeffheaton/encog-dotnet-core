using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Encog.Util.Randomize
{
    /// <summary>
    /// A randomizer that will create always set the random number to a const
    /// value, used mainly for testing.
    /// </summary>
    public class ConstRandomizer : BasicRandomizer
    {
        /// <summary>
        /// The constant value.
        /// </summary>
        private double value;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ConstRandomizer));

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        /// <param name="value">The constant value.</param>
        public ConstRandomizer(double value)
        {
            this.value = value;
        }


        /// <summary>
        /// Generate a random number based on the range specified in the constructor.
        /// </summary>
        /// <param name="d">The range randomizer ignores this value.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return this.value;
        }

    }

}
