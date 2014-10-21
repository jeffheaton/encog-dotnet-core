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
using Encog.Neural.Networks;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// A randomizer that takes a seed and will always produce consistent results.
    /// </summary>
    ///
    public class ConsistentRandomizer : BasicRandomizer
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
        /// The generator.
        /// </summary>
        ///
        private readonly LinearCongruentialGenerator _rand;

        /// <summary>
        /// The seed.
        /// </summary>
        ///
        private readonly int _seed;

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        ///
        /// <param name="min">The minimum random value.</param>
        /// <param name="max">The maximum random value.</param>
        public ConsistentRandomizer(double min, double max) : this(min,max, 1000)
        {
        }

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        ///
        /// <param name="min">The minimum random value.</param>
        /// <param name="max">The maximum random value.</param>
        /// <param name="seed">The seed value.</param>
        public ConsistentRandomizer(double min, double max,
                                    int seed)
        {
            _max = max;
            _min = min;
            _seed = seed;
            _rand = new LinearCongruentialGenerator(seed);
        }

        /// <summary>
        /// Generate a random number based on the range specified in the constructor.
        /// </summary>
        ///
        /// <param name="d">The range randomizer ignores this value.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return _rand.Range(_min, _max);
        }

        /// <summary>
        /// Randomize the network.
        /// </summary>
        ///
        /// <param name="network">The network to randomize.</param>
        public void Randomize(BasicNetwork network)
        {
            _rand.Seed = _seed;
            base.Randomize(network);
        }
    }
}
