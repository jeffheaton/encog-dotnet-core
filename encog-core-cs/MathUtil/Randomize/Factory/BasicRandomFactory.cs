using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.MathUtil.Randomize.Factory
{
    /// <summary>
    /// Basic random number generator factory.  Simply returns the Random class.
    /// </summary>
    [Serializable]
    public class BasicRandomFactory : IRandomFactory
    {
        /// <summary>
        /// A random generator to generate random seeds.
        /// </summary>
        private EncogRandom seedProducer;

        /// <summary>
        /// Construct a random generator factory. No assigned seed.
        /// </summary>
        public BasicRandomFactory()
        {
            this.seedProducer = new EncogRandom();
        }

        /// <summary>
        /// Construct a random generator factory with the specified seed. 
        /// </summary>
        /// <param name="theSeed">The seed.</param>
        public BasicRandomFactory(int theSeed)
        {
            this.seedProducer = new EncogRandom(theSeed);
        }

        /// <summary>
        /// Factor a new random generator.
        /// </summary>
        /// <returns>The random number generator.</returns>
        public EncogRandom Factor()
        {
            lock (this)
            {
                int seed = this.seedProducer.Next();
                return new EncogRandom(seed);
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
