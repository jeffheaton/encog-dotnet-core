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

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// Generally, you will not want to use this randomizer as a pure neural network
    /// randomizer. More on this later in the description.
    /// Generate random numbers that fall within a Gaussian curve. The mean
    /// represents the center of the curve, and the standard deviation helps
    /// determine the length of the curve on each side.
    /// This randomizer is used mainly for special cases where I want to generate
    /// random numbers in a Gaussian range. For a pure neural network initializer, it
    /// leaves much to be desired. However, it can make for a decent randomizer.
    /// Usually, the Nguyen Widrow randomizer performs better.
    /// Uses the "Box Muller" method.
    /// http://en.wikipedia.org/wiki/Box%E2%80%93Muller_transform
    /// Ported from C++ version provided by Everett F. Carter Jr., 1994
    /// </summary>
    [Serializable]
    public class GaussianRandomizer : BasicRandomizer
    {
        /// <summary>
        /// The mean.
        /// </summary>
        ///
        private readonly double _mean;

        /// <summary>
        /// The standard deviation.
        /// </summary>
        ///
        private readonly double _standardDeviation;

        /// <summary>
        /// Should we use the last value.
        /// </summary>
        ///
        private bool _useLast;

        /// <summary>
        /// The y2 value.
        /// </summary>
        ///
        private double _y2;

        /// <summary>
        /// Construct a Gaussian randomizer. The mean, the standard deviation.
        /// </summary>
        ///
        /// <param name="mean">The mean.</param>
        /// <param name="standardDeviation">The standard deviation.</param>
        public GaussianRandomizer(double mean, double standardDeviation)
        {
            _useLast = false;
            _mean = mean;
            _standardDeviation = standardDeviation;
        }

        /// <summary>
        /// Compute a Gaussian random number.
        /// </summary>
        ///
        /// <param name="m">The mean.</param>
        /// <param name="s">The standard deviation.</param>
        /// <returns>The random number.</returns>
        public double BoxMuller(double m, double s)
        {
            double y1;

            // use value from previous call
            if (_useLast)
            {
                y1 = _y2;
                _useLast = false;
            }
            else
            {
                double x1;
                double x2;
                double w;
                do
                {
                    x1 = 2.0d*NextDouble() - 1.0d;
                    x2 = 2.0d*NextDouble() - 1.0d;
                    w = x1*x1 + x2*x2;
                } while (w >= 1.0d);

                w = Math.Sqrt((-2.0d*Math.Log(w))/w);
                y1 = x1*w;
                _y2 = x2*w;
                _useLast = true;
            }

            return (m + y1*s);
        }

        /// <summary>
        /// Generate a random number.
        /// </summary>
        ///
        /// <param name="d">The input value, not used.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return BoxMuller(_mean, _standardDeviation);
        }
    }
}
