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
using Encog.Util.MathUtil;

namespace Encog.Util.Randomize
{
    /// <summary>
    /// A randomizer that takes a seed and will always produce consistent results.
    /// </summary>
    public class ConsistentRandomizer : BasicRandomizer
    {

        /// <summary>
        /// The generator.
        /// </summary>
        private LinearCongruentialGenerator rand;


        /// <summary>
        /// The minimum value for the random range.
        /// </summary>
        private double Min { get; set; }

        /// <summary>
        /// The maximum value for the random range.
        /// </summary>
        private double Max { get; set; }

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        /// <param name="min">The minimum random value.</param>
        /// <param name="max">The maximum random value.</param>
        public ConsistentRandomizer(double min, double max)
        {
            this.Max = max;
            this.Min = min;
            this.rand = new LinearCongruentialGenerator(1000);
        }

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        /// <param name="seed">The seed for the random number generator.</param>
        /// <param name="min">The minimum random value.</param>
        /// <param name="max">The maximum random value.</param>
        public ConsistentRandomizer(int seed, double min,
                 double max)
        {
            this.rand = new LinearCongruentialGenerator(seed);
            this.Max = max;
            this.Min = min;
        }

        /// <summary>
        /// Generate a random number based on the range specified in the constructor.
        /// </summary>
        /// <param name="d">The range randomizer ignores this value.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return rand.Range(this.Min, this.Max);
        }

    }
}
