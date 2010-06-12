// Encog(tm) Artificial Intelligence Framework v2.5
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

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// Generally, you will not want to use this randomizer as a pure neural network
    /// randomizer. More on this later in the description.
    /// 
    /// Generate random numbers that fall within a Gaussian curve. The mean
    /// represents the center of the curve, and the standard deviation helps
    /// determine the length of the curve on each side.
    /// 
    /// This randomizer is used mainly for special cases where I want to generate
    /// random numbers in a Gaussian range. For a pure neural network initilizer, it
    /// leaves much to be desired. Typically this randomizer provides the worst
    /// performance of any Encog randomizer.
    /// 
    /// Uses the "Box Muller" method.
    /// http://en.wikipedia.org/wiki/Box%E2%80%93Muller_transform
    /// 
    /// Ported from C++ version provided by Everett F. Carter Jr., 1994
    /// </summary>
    public class GaussianRandomizer : BasicRandomizer
    {
        /// <summary>
        /// The y2 value.
        /// </summary>
        private double y2;

        /// <summary>
        /// Should we use the last value.
        /// </summary>
        private bool useLast = false;

        /// <summary>
        /// The mean.
        /// </summary>
        private double mean;

        /// <summary>
        /// The standard deviation.
        /// </summary>
        private double standardDeviation;
        
        /// <summary>
        /// Compute a Gaussian random number. 
        /// </summary>
        /// <param name="m">The mean.</param>
        /// <param name="s">The standard deviation.</param>
        /// <returns>The random number.</returns>
        public double BoxMuller(double m, double s)
        {
            double x1, x2, w, y1;

            // use value from previous call
            if (useLast)
            {
                y1 = y2;
                useLast = false;
            }
            else
            {
                do
                {
                    x1 = 2.0 * ThreadSafeRandom.NextDouble() - 1.0;
                    x2 = 2.0 * ThreadSafeRandom.NextDouble() - 1.0;
                    w = x1 * x1 + x2 * x2;
                } while (w >= 1.0);

                w = Math.Sqrt((-2.0 * Math.Log(w)) / w);
                y1 = x1 * w;
                y2 = x2 * w;
                useLast = true;
            }

            return (m + y1 * s);
        }
        
        /// <summary>
        /// Construct a Gaussian randomizer.  The mean, the standard deviation. 
        /// </summary>
        /// <param name="mean">The mean.</param>
        /// <param name="standardDeviation">The standard deviation.</param>
        public GaussianRandomizer(double mean, double standardDeviation)
        {
            this.mean = mean;
            this.standardDeviation = standardDeviation;
        }

        /// <summary>
        /// Generate a random number.
        /// </summary>
        /// <param name="d">The input value, not used.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return BoxMuller(this.mean, this.standardDeviation);
        }

    }
}
