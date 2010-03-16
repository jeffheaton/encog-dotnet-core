using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;

namespace Encog.Solve.Genetic.Mutate
{
    public interface IMutate
    {
        /// <summary>
        /// Perform a mutation on the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to mutate.</param>
        void PerformMutation(Chromosome chromosome);
    }
}
