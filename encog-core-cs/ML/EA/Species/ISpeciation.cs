using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;
using Encog.ML.EA.Train;

namespace Encog.ML.EA.Species
{
    /// <summary>
    /// Defines a speciation strategy.
    /// </summary>
    public interface ISpeciation
    {
        /// <summary>
        /// Setup the speciation strategy.
        /// </summary>
        /// <param name="theOwner">The owner.</param>
        void Init(IEvolutionaryAlgorithm theOwner);

        /// <summary>
        /// Perform the speciation.
        /// </summary>
        /// <param name="genomeList">The genomes to speciate.</param>
        void PerformSpeciation(IList<IGenome> genomeList);
    }
}
