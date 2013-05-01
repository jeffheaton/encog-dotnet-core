using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.MathUtil.Randomize.Factory
{
    /// <summary>
    /// Basic random number generator factory.  Simply returns the Random class.
    /// </summary>
    public class BasicRandomFactory : IRandomFactory
    {
        /// <summary>
        /// A random generator to generate random seeds.
        /// </summary>
        private Random seedProducer;

        /// <summary>
        /// Construct a random generator factory. No assigned seed.
        /// </summary>
        public BasicRandomFactory()
        {
            this.seedProducer = new Random();
        }

        /// <summary>
        /// Construct a random generator factory with the specified seed. 
        /// </summary>
        /// <param name="theSeed">The seed.</param>
        public BasicRandomFactory(int theSeed)
        {
            this.seedProducer = new Random(theSeed);
        }

        /// <summary>
        /// Factor a new random generator.
        /// </summary>
        /// <returns>The random number generator.</returns>
        public Random Factor()
        {
            lock (this)
            {
                int seed = this.seedProducer.Next();
                return new Random(seed);
            }
        }

        /// <summary>
        /// Factor a new random generator factory.
        /// </summary>
        /// <returns>The random number generator generator.</returns>
        public IRandomFactory FactorFactory()
        {
            return new BasicRandomFactory(this.seedProducer.Next());
        }
    }
}
