using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;

namespace Encog.Solve.Genetic.Mutate
{
    public interface IMutate
    {
        /**
	 * Perform a mutation on the specified chromosome.
	 * @param chromosome The chromosome to mutate.
	 */
        void performMutation(Chromosome chromosome);
    }
}
