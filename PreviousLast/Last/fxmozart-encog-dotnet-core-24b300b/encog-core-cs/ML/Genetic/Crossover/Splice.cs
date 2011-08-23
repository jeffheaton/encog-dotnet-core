//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.ML.Genetic.Genome;
using Encog.MathUtil;

namespace Encog.ML.Genetic.Crossover
{
    /// <summary>
    /// A simple cross over where genes are simply "spliced". Genes are allowed to
    /// repeat.
    /// </summary>
    ///
    public class Splice : ICrossover
    {
        /// <summary>
        /// The cut length.
        /// </summary>
        ///
        private readonly int _cutLength;

        /// <summary>
        /// Create a slice crossover with the specified cut length.
        /// </summary>
        ///
        /// <param name="theCutLength">The cut length.</param>
        public Splice(int theCutLength)
        {
            _cutLength = theCutLength;
        }

        #region ICrossover Members

        /// <summary>
        /// Assuming this chromosome is the "mother" mate with the passed in
        /// "father".
        /// </summary>
        ///
        /// <param name="mother">The mother.</param>
        /// <param name="father">The father.</param>
        /// <param name="offspring1">Returns the first offspring</param>
        /// <param name="offspring2">Returns the second offspring.</param>
        public void Mate(Chromosome mother, Chromosome father,
                         Chromosome offspring1, Chromosome offspring2)
        {
            int geneLength = mother.Genes.Count;

            // the chromosome must be cut at two positions, determine them
            var cutpoint1 = (int)(ThreadSafeRandom.NextDouble()*(geneLength - _cutLength));
            int cutpoint2 = cutpoint1 + _cutLength;

            // handle cut section
            for (int i = 0; i < geneLength; i++)
            {
                if (!((i < cutpoint1) || (i > cutpoint2)))
                {
                    offspring1.GetGene(i).Copy(father.GetGene(i));
                    offspring2.GetGene(i).Copy(mother.GetGene(i));
                }
            }

            // handle outer sections
            for (int i = 0; i < geneLength; i++)
            {
                if ((i < cutpoint1) || (i > cutpoint2))
                {
                    offspring1.GetGene(i).Copy(mother.GetGene(i));
                    offspring2.GetGene(i).Copy(father.GetGene(i));
                }
            }
        }

        #endregion
    }
}
