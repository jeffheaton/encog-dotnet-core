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

namespace Encog.ML.Genetic.Crossover
{
    /// <summary>
    /// A simple cross over where genes are simply "spliced". Genes are allowed to
    /// repeat.
    /// </summary>
    public class Splice : IEvolutionaryOperator
    {
        /// <summary>
        /// The cut length.
        /// </summary>
        private int cutLength;

        /// <summary>
        /// The owner.
        /// </summary>
        private IEvolutionaryAlgorithm owner;

        /// <summary>
        ///  Create a slice crossover with the specified cut length. 
        /// </summary>
        /// <param name="theCutLength">The cut length.</param>
        public Splice(int theCutLength)
        {
            this.cutLength = theCutLength;
        }

        /// <inheritdoc/>
        public void PerformOperation(EncogRandom rnd, IGenome[] parents, int parentIndex,
                IGenome[] offspring, int offspringIndex)
        {

            IArrayGenome mother = (IArrayGenome)parents[parentIndex];
            IArrayGenome father = (IArrayGenome)parents[parentIndex + 1];
            IArrayGenome offspring1 = (IArrayGenome)this.owner.Population.GenomeFactory.Factor();
            IArrayGenome offspring2 = (IArrayGenome)this.owner.Population.GenomeFactory.Factor();

            offspring[offspringIndex] = offspring1;
            offspring[offspringIndex + 1] = offspring2;

            int geneLength = mother.Size;

            // the chromosome must be cut at two positions, determine them
            int cutpoint1 = (int)(rnd.Next(geneLength - this.cutLength));
            int cutpoint2 = cutpoint1 + this.cutLength;

            // handle cut section
            for (int i = 0; i < geneLength; i++)
            {
                if (!((i < cutpoint1) || (i > cutpoint2)))
                {
                    offspring1.Copy(father, i, i);
                    offspring2.Copy(mother, i, i);
                }
            }

            // handle outer sections
            for (int i = 0; i < geneLength; i++)
            {
                if ((i < cutpoint1) || (i > cutpoint2))
                {
                    offspring1.Copy(mother, i, i);
                    offspring2.Copy(father, i, i);
                }
            }
        }

        /// <inheritdoc/>
        public int OffspringProduced
        {
            get
            {
                return 2;
            }
        }

        /// <inheritdoc/>
        public int ParentsNeeded
        {
            get
            {
                return 2;
            }
        }

        /// <inheritdoc/>
        public void Init(IEvolutionaryAlgorithm theOwner)
        {
            this.owner = theOwner;

        }
    }
}
