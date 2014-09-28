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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.MathUtil.Randomize.Factory
{
    /// <summary>
    /// Basic random number generator factory.  Simply returns the Random class.
    /// </summary>
    [Serializable]
    public class BasicRandomFactory : IRandomFactory
    {
        /// <summary>
        /// A random generator to generate random seeds.
        /// </summary>
        private EncogRandom seedProducer;

        /// <summary>
        /// Construct a random generator factory. No assigned seed.
        /// </summary>
        public BasicRandomFactory()
        {
            this.seedProducer = new EncogRandom();
        }

        /// <summary>
        /// Construct a random generator factory with the specified seed. 
        /// </summary>
        /// <param name="theSeed">The seed.</param>
        public BasicRandomFactory(int theSeed)
        {
            this.seedProducer = new EncogRandom(theSeed);
        }

        /// <summary>
        /// Factor a new random generator.
        /// </summary>
        /// <returns>The random number generator.</returns>
        public EncogRandom Factor()
        {
            lock (this)
            {
                int seed = this.seedProducer.Next();
                return new EncogRandom(seed);
            }
        }

        /// <summary>
        /// Factor a new random generator factory.
        /// </summary>
        /// <returns>The random number generator generator.</returns>
        public IRandomFactory FactorFactory()
        {
            return new BasicRandomFactory(this.seedProducer.Next());
        }
    }
}
