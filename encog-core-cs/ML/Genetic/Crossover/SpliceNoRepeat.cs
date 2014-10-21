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
using Encog.ML.Genetic.Genome;
using Encog.ML.EA.Genome;
using Encog.MathUtil.Randomize;

namespace Encog.ML.Genetic.Crossover
{
    /// <summary>
    /// A simple cross over where genes are simply "spliced". Genes are not allowed
    /// to repeat.  This method only works with IntegerArrayGenome.
    /// </summary>
    public class SpliceNoRepeat : IEvolutionaryOperator
    {
        /// <summary>
        /// The owner.
        /// </summary>
        private IEvolutionaryAlgorithm owner;

        /// <summary>
        /// Get a list of the genes that have not been taken before. This is useful
        /// if you do not wish the same gene to appear more than once in a
        /// genome.
        /// </summary>
        /// <param name="source">The pool of genes to select from.</param>
        /// <param name="taken"> An array of the taken genes.</param>
        /// <returns>Those genes in source that are not taken.</returns>
        private static int GetNotTaken(IntegerArrayGenome source,
                HashSet<int> taken)
        {

            foreach (int trial in source.Data)
            {
                if (!taken.Contains(trial))
                {
                    taken.Add(trial);
                    return trial;
                }
            }

            throw new GeneticError("Ran out of integers to select.");
        }

        /// <summary>
        /// The cut length.
        /// </summary>
        private int cutLength;

        /**
         * Construct a splice crossover.
         * 
         * @param theCutLength
         *            The cut length.
         */
        public SpliceNoRepeat(int theCutLength)
        {
            this.cutLength = theCutLength;
        }

        /// <inheritdoc/>
        public void PerformOperation(EncogRandom rnd, IGenome[] parents, int parentIndex,
                IGenome[] offspring, int offspringIndex)
        {

            IntegerArrayGenome mother = (IntegerArrayGenome)parents[parentIndex];
            IntegerArrayGenome father = (IntegerArrayGenome)parents[parentIndex + 1];
            IntegerArrayGenome offspring1 = (IntegerArrayGenome)this.owner.Population.GenomeFactory.Factor();
            IntegerArrayGenome offspring2 = (IntegerArrayGenome)this.owner.Population.GenomeFactory.Factor();

            offspring[offspringIndex] = offspring1;
            offspring[offspringIndex + 1] = offspring2;

            int geneLength = mother.Size;

            // the chromosome must be cut at two positions, determine them
            int cutpoint1 = (int)(rnd.Next(geneLength - this.cutLength));
            int cutpoint2 = cutpoint1 + this.cutLength;

            // keep track of which genes have been taken in each of the two
            // offspring, defaults to false.
            HashSet<int> taken1 = new HashSet<int>();
            HashSet<int> taken2 = new HashSet<int>();

            // handle cut section
            for (int i = 0; i < geneLength; i++)
            {
                if (!((i < cutpoint1) || (i > cutpoint2)))
                {
                    offspring1.Copy(father, i, i);
                    offspring2.Copy(mother, i, i);
                    taken1.Add(father.Data[i]);
                    taken2.Add(mother.Data[i]);
                }
            }

            // handle outer sections
            for (int i = 0; i < geneLength; i++)
            {
                if ((i < cutpoint1) || (i > cutpoint2))
                {

                    offspring1.Data[i] = SpliceNoRepeat.GetNotTaken(mother, taken1);
                    offspring2.Data[i] = SpliceNoRepeat.GetNotTaken(father, taken2);

                }
            }
        }

        /// <summary>
        /// The number of offspring produced, which is 2 for splice crossover.
        /// </summary>
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
