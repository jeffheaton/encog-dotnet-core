//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;

namespace Encog.MathUtil
{
    /// <summary>
    /// A predictable random number generator. This is useful for unit tests and
    /// benchmarks where we want random numbers, but we want them to be the same each
    /// time. This class exists on both Java and C# so it can even provide consistent
    /// random numbers over the two platforms.
    /// 
    /// Random numbers are created using a LCG.
    /// 
    /// http://en.wikipedia.org/wiki/Linear_congruential_generator
    ///
    /// </summary>
    public class LinearCongruentialGenerator
    {
        /// <summary>
        /// The maximum rand number that the standard GCC based LCG will generate.
        /// </summary>
        public const long MaxRand = 4294967295L;

        /// <summary>
        /// Construct the default LCG.  You need only specify a seed.
        /// </summary>
        /// <param name="seed">The seed.</param>
        public LinearCongruentialGenerator(long seed)
            : this((long) Math.Pow(2L, 32L), 1103515245L, 12345L, seed)
        {
        }

        /// <summary>
        /// Create a LCG with the specified modulus, multiplier and increment. Unless
        /// you REALLY KNOW WHAT YOU ARE DOING, just use the constructor that just
        /// takes a seed. It will set these values to the same as set by the GCC C
        /// compiler. Setting these values wrong can create fairly useless random
        /// numbers.
        /// </summary>
        /// <param name="modulus">The modulus for the LCG algorithm.</param>
        /// <param name="multiplier">The multiplier for the LCG algorithm.</param>
        /// <param name="increment">The increment for the LCG algorithm.</param>
        /// <param name="seed">The seed for the LCG algorithm. Using the same seed will give
        /// the same random number sequence each time, whether in Java or
        /// DotNet.</param>
        public LinearCongruentialGenerator(long modulus,
                                           long multiplier, long increment, long seed)
        {
            Modulus = modulus;
            Multiplier = multiplier;
            Increment = increment;
            Seed = seed;
        }

        /// <summary>
        /// The modulus.
        /// </summary>
        public long Modulus { get; set; }

        /// <summary>
        /// The multiplier.
        /// </summary>
        public long Multiplier { get; set; }

        /// <summary>
        /// The amount to increment.
        /// </summary>
        public long Increment { get; set; }

        /// <summary>
        /// The current seed, set to an initial value and always holds the value of
        /// the last random number generated.
        /// </summary>
        public long Seed { get; set; }


        /// <summary>
        /// The next random number as a double between 0 and 1.
        /// </summary>
        /// <returns>The next double.</returns>
        public double NextDouble()
        {
            return (double) NextLong()/MaxRand;
        }

        /// <summary>
        /// The next random number as a long between 0 and MAX_RAND.
        /// </summary>
        /// <returns>The next long.</returns>
        public long NextLong()
        {
            Seed = (Multiplier*Seed + Increment)
                   %Modulus;
            return Seed;
        }

        /// <summary>
        /// Generate a random number in the specified range.
        /// </summary>
        /// <param name="min">The minimum random number.</param>
        /// <param name="max">The maximum random number.</param>
        /// <returns>The generated random number.</returns>
        public double Range(double min, double max)
        {
            double range = max - min;
            return (range*NextDouble()) + min;
        }
    }
}
