using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Concurrency;

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


        /// <summary>
        /// Mate the two chromosomes.
        /// </summary>
        public void Run()
        {
            this.mother.Mate(this.father, this.child1, this.child2);
        }

    }

}
