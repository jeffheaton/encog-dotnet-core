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

#if logging
using log4net;
#endif

namespace Encog.Util.Randomize
{
    /// <summary>
    /// A randomizer that will create random weight and threshold values that are
    /// between a specified range.
    /// </summary>
    public class RangeRandomizer : BasicRandomizer
    {
        /// <summary>
        /// Generate a random number in the specified range.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A random number.</returns>
        public static double Randomize(double min, double max)
        {
            double range = max - min;
            return (range * ThreadSafeRandom.NextDouble()) + min;
        }

        /// <summary>
        /// The minimum value for the random range.
        /// </summary>
        private double min;

        /// <summary>
        /// The maximum value for the random range.
        /// </summary>
        private double max;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(RangeRandomizer));
#endif

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        /// <param name="min">The minimum random value.</param>
        /// <param name="max">The maximum random value.</param>
        public RangeRandomizer(double min, double max)
        {
            this.max = max;
            this.min = min;
        }

        /// <summary>
        /// Generate a random number based on the range specified in the constructor.
        /// </summary>
        /// <param name="d">The range randomizer ignores this value.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return RangeRandomizer.Randomize(this.min, this.max);
        }

    }

}
