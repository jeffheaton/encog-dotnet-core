using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Concurrency;
using Encog.Solve.Genetic.Genome;

namespace Encog.Solve.Genetic
{
    /// <summary>
    /// Worker that is used to mate the genomes in a multithreaded way.
    /// </summary>
    public class MateWorker : IEncogTask
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
