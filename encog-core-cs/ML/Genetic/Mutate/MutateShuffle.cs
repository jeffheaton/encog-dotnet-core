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
using Encog.ML.EA.Opp;
using Encog.ML.EA.Opp.Selection;
using Encog.ML.EA.Train;
using Encog.ML.EA.Genome;
using Encog.ML.Genetic.Genome;
using Encog.MathUtil.Randomize;

namespace Encog.ML.Genetic.Mutate
{
    /// <summary>
    /// A simple mutation where genes are shuffled.
    /// This mutation will not produce repeated genes.
    /// </summary>
    public class MutateShuffle : IEvolutionaryOperator
    {
        /// <summary>
        /// The owner.
        /// </summary>
        private IEvolutionaryAlgorithm owner;

        /// <inheritdoc/>
        public void PerformOperation(EncogRandom rnd, IGenome[] parents, int parentIndex,
                IGenome[] offspring, int offspringIndex)
        {
            IArrayGenome parent = (IArrayGenome)parents[parentIndex];
            offspring[offspringIndex] = this.owner.Population.GenomeFactory.Factor();
            IArrayGenome child = (IArrayGenome)offspring[offspringIndex];

            child.Copy(parent);

            int length = parent.Size;
            int iswap1 = (int)(rnd.NextDouble() * length);
            int iswap2 = (int)(rnd.NextDouble() * length);

            // can't be equal
            if (iswap1 == iswap2)
            {
                // move to the next, but
                // don't go out of bounds
                if (iswap1 > 0)
                {
                    iswap1--;
                }
                else
                {
                    iswap1++;
                }

            }

            // make sure they are in the right order
            if (iswap1 > iswap2)
            {
                int temp = iswap1;
                iswap1 = iswap2;
                iswap2 = temp;
            }

            child.Swap(iswap1, iswap2);
        }

        /// <summary>
        /// The number of offspring produced, which is 1 for this mutation.
        /// </summary>
        public int OffspringProduced
        {
            get
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public int ParentsNeeded
        {
            get
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public void Init(IEvolutionaryAlgorithm theOwner)
        {
            this.owner = theOwner;
        }
    }
}
