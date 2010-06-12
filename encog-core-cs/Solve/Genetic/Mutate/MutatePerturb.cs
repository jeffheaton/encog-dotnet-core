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
using Encog.MathUtil;
using Encog.Solve.Genetic.Genome;

namespace Encog.Solve.Genetic.Mutate
{
    /// <summary>
    /// A simple mutation based on random numbers.
    /// </summary>
    public class MutatePerturb : IMutate
    {
        /// <summary>
        /// The amount to perturb by.
        /// </summary>
        private double perturbAmount;

        /// <summary>
        /// Construct a perturb mutation.
        /// </summary>
        /// <param name="perturbAmount">The amount to mutate by(percent).</param>
        public MutatePerturb(double perturbAmount)
        {
            this.perturbAmount = perturbAmount;
        }


        /// <summary>
        /// Perform a perturb mutation on the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to mutate.</param>
        public void PerformMutation(Chromosome chromosome)
        {
            foreach (IGene gene in chromosome.Genes)
            {
                if (gene is DoubleGene)
                {
                    DoubleGene doubleGene = (DoubleGene)gene;
                    double value = doubleGene.Value;
                    value += (perturbAmount - (ThreadSafeRandom.NextDouble()) * perturbAmount * 2);
                    doubleGene.Value = value;
                }
            }
        }
    }
}
