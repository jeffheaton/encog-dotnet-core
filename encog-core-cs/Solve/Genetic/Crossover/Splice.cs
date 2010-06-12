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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;
using Encog.MathUtil;

namespace Encog.Solve.Genetic.Crossover
{
    /// <summary>
    /// A simple cross over where genes are simply "spliced".
    /// Genes are allowed to repeat.
    /// </summary>
    public class Splice : ICrossover
    {
        /// <summary>
        /// The cut length.
        /// </summary>
        private int cutLength;

        /// <summary>
        /// Construct a splice cross over object.
        /// </summary>
        /// <param name="cutLength"></param>
        public Splice(int cutLength)
        {
            this.cutLength = cutLength;
        }

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
            int geneLength = mother.Genes.Count;

            // the chromosome must be cut at two positions, determine them
            int cutpoint1 = (int)(ThreadSafeRandom.NextDouble() * (geneLength - cutLength));
            int cutpoint2 = cutpoint1 + cutLength;

            // handle cut section
            for (int i = 0; i < geneLength; i++)
            {
                if (!((i < cutpoint1) || (i > cutpoint2)))
                {
                    offspring1.Genes[i].Copy(father.Genes[i]);
                    offspring2.Genes[i].Copy(mother.Genes[i]);
                }
            }

            // handle outer sections
            for (int i = 0; i < geneLength; i++)
            {
                if ((i < cutpoint1) || (i > cutpoint2))
                {
                    offspring1.Genes[i].Copy(mother.Genes[i]);
                    offspring2.Genes[i].Copy(father.Genes[i]);
                }
            }
        }
    }
}
