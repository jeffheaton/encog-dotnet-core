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
    /// A randomizer that will create always set the random number to a const value,
    /// used mainly for testing.
    /// </summary>
    ///
    public class ConstRandomizer : BasicRandomizer
    {
        /// <summary>
        /// The constant value.
        /// </summary>
        ///
        private readonly double value;

        /// <summary>
        /// Construct a range randomizer.
        /// </summary>
        ///
        /// <param name="v">The constant value.</param>
        public ConstRandomizer(double v)
        {
            this.value = v;
        }

        /// <summary>
        /// Generate a random number based on the range specified in the constructor.
        /// </summary>
        ///
        /// <param name="d">The range randomizer ignores this value.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return value;
        }
    }
}
