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
using Encog.ML.EA.Genome;

namespace Encog.ML.Genetic.Genome
{
    /// <summary>
    /// A factory to create integer genomes of a specific size.
    /// </summary>
    public class IntegerArrayGenomeFactory : IGenomeFactory
    {
        /// <summary>
        /// The size of genome to create.
        /// </summary>
        private int size;

        /// <summary>
        /// Create the integer genome of a fixed size.
        /// </summary>
        /// <param name="theSize">The size to use.</param>
        public IntegerArrayGenomeFactory(int theSize)
        {
            this.size = theSize;
        }

        /// <inheritdoc/>
        public IGenome Factor()
        {
            return new IntegerArrayGenome(this.size);
        }

        /// <inheritdoc/>
        public IGenome Factor(IGenome other)
        {
            return new IntegerArrayGenome(((IntegerArrayGenome)other));
        }
    }
}
