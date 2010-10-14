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
using Encog.Engine.Concurrency.Job;

namespace Encog.Solve.Genetic
{
    /// <summary>
    /// Worker that is used to mate the genomes in a multithreaded way.
    /// </summary>
    public class MateWorker : IEngineTask
    {
        /// <summary>
        /// The first child.
        /// </summary>
        private IGenome child1;

        /// <summary>
        /// The second child.
        /// </summary>
        private IGenome child2;

        /// <summary>
        /// The father.
        /// </summary>
        private IGenome father;

        /// <summary>
        /// The mother.
        /// </summary>
        private IGenome mother;


        /// <summary>
        /// Create a MateWorker.
        /// </summary>
        /// <param name="mother">The mother.</param>
        /// <param name="father">The father.</param>
        /// <param name="child1">The first child.</param>
        /// <param name="child2">The second child.</param>
        public MateWorker(IGenome mother, IGenome father,
                IGenome child1, IGenome child2)
        {
            this.mother = mother;
            this.father = father;
            this.child1 = child1;
            this.child2 = child2;
        }

        /// <summary>
        /// Mate the two chromosomes.
        /// </summary>
        public void Run()
        {
            mother.Mate(father, child1, child2);
        }
    }
}
