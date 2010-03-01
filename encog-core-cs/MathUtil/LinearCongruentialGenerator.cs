// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// The maximum rand number that the standard GCC based LCG will generate.
        /// </summary>
        public const long MAX_RAND = 4294967295L;

        /// <summary>
        /// Construct the default LCG.  You need only specify a seed.
        /// </summary>
        /// <param name="seed">The seed.</param>
        public LinearCongruentialGenerator(long seed)
            : this((long)Math.Pow(2L, 32L), 1103515245L, 12345L, seed)
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
            this.Modulus = modulus;
            this.Multiplier = multiplier;
            this.Increment = increment;
            this.Seed = seed;
        }


        /// <summary>
        /// The next random number as a double between 0 and 1.
        /// </summary>
        /// <returns>The next double.</returns>
        public double NextDouble()
        {
            return (double)NextLong() / LinearCongruentialGenerator.MAX_RAND;
        }

        /// <summary>
        /// The next random number as a long between 0 and MAX_RAND.
        /// </summary>
        /// <returns>The next long.</returns>
        public long NextLong()
        {
            this.Seed = (this.Multiplier * this.Seed + this.Increment)
                    % this.Modulus;
            return this.Seed;
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
            return (range * NextDouble()) + min;
        }

    }
}
