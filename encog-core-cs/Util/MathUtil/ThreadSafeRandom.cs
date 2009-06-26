using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.MathUtil
{
    /// <summary>
    /// A thread safe random number generator.
    /// </summary>
    public class ThreadSafeRandom
    {
        /// <summary>
        /// A non-thread-safe random number generator that uses a time-based seed.
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// Generate a random number between 0 and 1.
        /// </summary>
        /// <returns></returns>
        public static double NextDouble()
        {
            lock (random)
            {
                return random.NextDouble();
            }
        }
    }
}
