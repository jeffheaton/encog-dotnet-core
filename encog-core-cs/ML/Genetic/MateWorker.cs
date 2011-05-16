using Encog.ML.Genetic.Genome;
using Encog.Util.Concurrency;

namespace Encog.ML.Genetic
{
    /// <summary>
    /// This class is used in conjunction with a thread pool. This allows the genetic
    /// algorithm to offload all of those calculations to a thread pool.
    /// </summary>
    ///
    public class MateWorker : IEngineTask
    {
        /// <summary>
        /// The first child.
        /// </summary>
        ///
        private readonly IGenome child1;

        /// <summary>
        /// The second child.
        /// </summary>
        ///
        private readonly IGenome child2;

        /// <summary>
        /// The father.
        /// </summary>
        ///
        private readonly IGenome father;

        /// <summary>
        /// The mother.
        /// </summary>
        ///
        private readonly IGenome mother;


        /// <param name="theMother">The mother.</param>
        /// <param name="theFather">The father.</param>
        /// <param name="theChild1">The first child.</param>
        /// <param name="theChild2">The second child.</param>
        public MateWorker(IGenome theMother, IGenome theFather,
                          IGenome theChild1, IGenome theChild2)
        {
            mother = theMother;
            father = theFather;
            child1 = theChild1;
            child2 = theChild2;
        }

        #region IEngineTask Members

        /// <summary>
        /// Mate the two chromosomes.
        /// </summary>
        ///
        public void Run()
        {
            mother.Mate(father, child1, child2);
        }

        #endregion
    }
}