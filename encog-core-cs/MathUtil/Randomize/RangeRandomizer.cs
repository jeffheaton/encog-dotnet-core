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
namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// A randomizer that will create random weight and bias values that are between
    /// a specified range.
    /// </summary>
    ///
    public class RangeRandomizer : BasicRandomizer
    {
        /// <summary>
        /// The maximum value for the random range.
        /// </summary>
        ///
        private readonly double _max;

        /// <summary>
        /// The minimum value for the random range.
        /// </summary>
        ///
        private readonly double _min;

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        ///
        /// <param name="min">The minimum random value.</param>
        /// <param name="max">The maximum random value.</param>
        public RangeRandomizer(double min, double max)
        {
            _max = max;
            _min = min;
        }


        /// <value>the min</value>
        public double Min
        {
            get { return _min; }
        }


        /// <value>the max</value>
        public double Max
        {
            get { return _max; }
        }

        /// <summary>
        /// Produce a random int, within a specified range.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The random int.</returns>
        public static int RandomInt(int min, int max)
        {
            return (int) Randomize(min, max + 1);
        }

        /// <summary>
        /// Generate a random number in the specified range.
        /// </summary>
        ///
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A random number.</returns>
        public static double Randomize(double min, double max)
        {
            double range = max - min;
            return (range*ThreadSafeRandom.NextDouble()) + min;
        }

        /// <summary>
        /// Generate a random number in the specified range.
        /// </summary>
        /// <param name="rnd">Random number generator.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A random number.</returns>
        public static double Randomize(EncogRandom rnd, double min, double max)
        {
            double range = max - min;
            return (range * rnd.NextDouble()) + min;
        }

        /// <summary>
        /// Generate a random number based on the range specified in the constructor.
        /// </summary>
        ///
        /// <param name="d">The range randomizer ignores this value.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return NextDouble(_min, _max);
        }
    }
}
