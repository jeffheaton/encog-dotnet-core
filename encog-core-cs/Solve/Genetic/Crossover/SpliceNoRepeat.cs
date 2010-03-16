using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genes;
using Encog.Solve.Genetic.Genome;
using Encog.MathUtil;

namespace Encog.Solve.Genetic.Crossover
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
        private int cutLength;

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
            int geneLength = source.getGenes().Count;

            for (int i = 0; i < geneLength; i++)
            {
                IGene trial = source.getGene(i);

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

        /// <summary>
        /// Construct a splice crossover.
        /// </summary>
        /// <param name="cutLength">The cut length.</param>
        public SpliceNoRepeat(int cutLength)
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
            int geneLength = father.getGenes().Count;

            // the chromosome must be cut at two positions, determine them
            int cutpoint1 = (int)(ThreadSafeRandom.NextDouble() * (geneLength - cutLength));
            int cutpoint2 = cutpoint1 + cutLength;

            // keep track of which cities have been taken in each of the two
            // offspring, defaults to false.
            IList<IGene> taken1 = new List<IGene>();
            IList<IGene> taken2 = new List<IGene>();

            // handle cut section
            for (int i = 0; i < geneLength; i++)
            {
                if (!((i < cutpoint1) || (i > cutpoint2)))
                {
                    offspring1.getGene(i).Copy(father.getGene(i));
                    offspring2.getGene(i).Copy(mother.getGene(i));
                    taken1.Add(offspring1.getGene(i));
                    taken2.Add(offspring2.getGene(i));
                }
            }

            // handle outer sections
            for (int i = 0; i < geneLength; i++)
            {
                if ((i < cutpoint1) || (i > cutpoint2))
                {

                    offspring1.getGene(i).Copy(
                            SpliceNoRepeat.GetNotTaken(mother, taken1));
                    offspring2.getGene(i).Copy(
                            SpliceNoRepeat.GetNotTaken(father, taken2));

                }
            }
        }
    }
}
