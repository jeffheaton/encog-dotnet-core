// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using Encog.Neural.Genetic;

namespace Encog.Solve.Genetic
{
    class MateWorker<GENE_TYPE>
    {
        private Chromosome<GENE_TYPE> mother;
        private Chromosome<GENE_TYPE> father;
        private Chromosome<GENE_TYPE> child1;
        private Chromosome<GENE_TYPE> child2;
        private ManualResetEvent eventHandler;

        /// <summary>
        /// Construct a MateWorker class to handle one mating.
        /// </summary>
        /// <param name="mother">The mother to mate.</param>
        /// <param name="father">The father to mate.</param>
        /// <param name="child1">The first offspring.</param>
        /// <param name="child2">The second offspring.</param>
        public MateWorker(Chromosome<GENE_TYPE> mother, Chromosome<GENE_TYPE> father,
             Chromosome<GENE_TYPE> child1, Chromosome<GENE_TYPE> child2)
        {
            this.mother = mother;
            this.father = father;
            this.child1 = child1;
            this.child2 = child2;
        }

        /// <summary>
        /// Set the event that will be used to signal when the mating is complete.
        /// </summary>
        /// <param name="eventHandler">The event object to use.</param>
        public void SetEvent(ManualResetEvent eventHandler)
        {
            this.eventHandler = eventHandler;
        }

        /// <summary>
        /// Perform the mating.
        /// </summary>
        public void Call()
        {
            this.mother.Mate(this.father,
                    this.child1,
                    this.child2);
            if (this.eventHandler != null)
            {
                this.eventHandler.Set();
            }
        }
    }
}
