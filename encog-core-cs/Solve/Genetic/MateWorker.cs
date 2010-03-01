// Encog(tm) Artificial Intelligence Framework v2.3
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
using Encog.MathUtil.Concurrency;

namespace Encog.Solve.Genetic
{
    /// <summary>
    /// This class is used in conjunction with a thread pool. This allows
    /// the genetic algorithm to offload all of those calculations to a thread pool.
    /// </summary>
    /// <typeparam name="GENE_TYPE">The data type of the gene.</typeparam>
    public class MateWorker<GENE_TYPE> :
            IEncogTask
    {

        /// <summary>
        /// The mother.
        /// </summary>
        private Chromosome<GENE_TYPE> mother;

        /// <summary>
        /// The father.
        /// </summary>
        private Chromosome<GENE_TYPE> father;

        /// <summary>
        /// The first child.
        /// </summary>
        private Chromosome<GENE_TYPE> child1;

        /// <summary>
        /// The second child.
        /// </summary>
        private Chromosome<GENE_TYPE> child2;


        /// <summary>
        /// Construct a worker for the thread pool.
        /// </summary>
        /// <param name="mother">The mother.</param>
        /// <param name="father">The father.</param>
        /// <param name="child1">The first offspring.</param>
        /// <param name="child2">The second offspring.</param>
        public MateWorker(Chromosome<GENE_TYPE> mother,
                 Chromosome<GENE_TYPE> father,
                 Chromosome<GENE_TYPE> child1,
                 Chromosome<GENE_TYPE> child2)
        {
            this.mother = mother;
            this.father = father;
            this.child1 = child1;
            this.child2 = child2;
        }

        static int count = 0;

        /// <summary>
        /// Mate the two chromosomes.
        /// </summary>
        public void Run()
        {
            count++;
            this.mother.Mate(this.father, this.child1, this.child2);
            
        }

    }

}
