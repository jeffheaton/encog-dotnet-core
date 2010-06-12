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
using Encog.Solve.Genetic.Genes;
using Encog.Solve.Genetic.Genome;
using Encog.MathUtil;

namespace Encog.Solve.Genetic.Mutate
{
    /// <summary>
    /// A simple mutation where genes are shuffled.
    /// This mutation will not produce repeated genes.
    /// </summary>
    public class MutateShuffle : IMutate
    {
        /// <summary>
        /// Perform a shuffle mutation.
        /// </summary>
        /// <param name="chromosome">The chromosome to mutate.</param>
        public void PerformMutation(Chromosome chromosome)
        {
            int length = chromosome.Genes.Count;
            int iswap1 = (int)(ThreadSafeRandom.NextDouble() * length);
            int iswap2 = (int)(ThreadSafeRandom.NextDouble() * length);

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

            IGene gene1 = chromosome.Genes[iswap1];
            IGene gene2 = chromosome.Genes[iswap2];

            // remove the two genes
            chromosome.Genes.Remove(gene1);
            chromosome.Genes.Remove(gene2);

            // insert them back in, reverse order
            chromosome.Genes.Insert(iswap1, gene2);
            chromosome.Genes.Insert(iswap2, gene1);
        }
    }
}
