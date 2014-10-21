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
    /// A randomizer that distorts what is already present in the neural network.
    /// </summary>
    ///
    public class Distort : BasicRandomizer
    {
        /// <summary>
        /// The factor to use to distort the numbers.
        /// </summary>
        ///
        private readonly double _factor;

        /// <summary>
        /// Construct a distort randomizer for the specified factor.
        /// </summary>
        ///
        /// <param name="f">The randomizer factor.</param>
        public Distort(double f)
        {
            _factor = f;
        }

        /// <summary>
        /// Distort the random number by the factor that was specified in the
        /// constructor.
        /// </summary>
        ///
        /// <param name="d">The number to distort.</param>
        /// <returns>The result.</returns>
        public override double Randomize(double d)
        {
            return d + (_factor - (NextDouble()*_factor*2));
        }
    }
}
