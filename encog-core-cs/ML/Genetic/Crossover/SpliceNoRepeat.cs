// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System.Collections.Generic;
using Encog.MathUtil;
using Encog.ML.Genetic.Genes;
using Encog.ML.Genetic.Genome;

namespace Encog.ML.Genetic.Crossover
{
    /// <summary>
    /// A simple cross over where genes are simply "spliced".
    /// Genes are not allowed to repeat.
    /// </summary>
    public class SpliceNoRepeat : ICrossover
    {
        /// <summary>
        /// The cut length.
        /// </summary>
        private readonly int cutLength;

        /// <summary>
        /// Construct a splice crossover.
        /// </summary>
        /// <param name="cutLength">The cut length.</param>
        public SpliceNoRepeat(int cutLength)
        {
            this.cutLength = cutLength;
        }

        #region ICrossover Members

        /// <summary>
        /// Assuming this chromosome is the "mother" mate with the passed in
        /// "father".
        /// </summary>
        /// <param name="mother">The mother.</param>
        /// <param name="father">The father.</param>
        /// <param name="offspring1">The first offspring.</param>
        /// <param name="offspring2">The second offspring.</param>
        public void Mate(Chromosome mother, Chromosome father,
                         Chromosome offspring1, Chromosome offspring2)
        {
            int geneLength = father.Genes.Count;

            // the chromosome must be cut at two positions, determine them
            var cutpoint1 = (int) (ThreadSafeRandom.NextDouble()*(geneLength - cutLength));
            int cutpoint2 = cutpoint1 + cutLength;

            // keep track of which genes have been taken in each of the two
            // offspring, defaults to false.
            IList<IGene> taken1 = new List<IGene>();
            IList<IGene> taken2 = new List<IGene>();

            // handle cut section
            for (int i = 0; i < geneLength; i++)
            {
                if (!((i < cutpoint1) || (i > cutpoint2)))
                {
                    offspring1.Genes[i].Copy(father.Genes[i]);
                    offspring2.Genes[i].Copy(mother.Genes[i]);
                    taken1.Add(offspring1.Genes[i]);
                    taken2.Add(offspring2.Genes[i]);
                }
            }

            // handle outer sections
            for (int i = 0; i < geneLength; i++)
            {
                if ((i < cutpoint1) || (i > cutpoint2))
                {
                    offspring1.Genes[i].Copy(
                        GetNotTaken(mother, taken1));
                    offspring2.Genes[i].Copy(
                        GetNotTaken(father, taken2));
                }
            }
        }

        #endregion

        /// <summary>
        /// Get a list of the genes that have not been taken before. This is useful
        /// if you do not wish the same gene to appear more than once in a
        /// chromosome.
        /// </summary>
        /// <param name="source">The pool of genes to select from.</param>
        /// <param name="taken">An array of the taken genes.</param>
        /// <returns>Those genes in source that are not taken.</returns>
        private static IGene GetNotTaken(Chromosome source,
                                         IList<IGene> taken)
        {
            int geneLength = source.Genes.Count;

            for (int i = 0; i < geneLength; i++)
            {
                IGene trial = source.Genes[i];

                bool found = false;
                foreach (IGene current in taken)
                {
                    if (current.Equals(trial))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    taken.Add(trial);
                    return trial;
                }
            }

            return null;
        }
    }
}