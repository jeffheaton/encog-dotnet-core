using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Species;
using Encog.ML.EA.Train;

namespace Encog.ML.EA.Opp.Selection
{
    /// <summary>
    /// Provides the interface to a selection operator. This allows genomes to be
    /// selected for offspring production or elimination.
    /// </summary>
    public interface ISelectionOperator
    {
        /// <summary>
        /// Selects an fit genome.
        /// </summary>
        /// <param name="rnd">A random number generator.</param>
        /// <param name="species">The species to select the genome from.</param>
        /// <returns>The selected genome.</returns>
        int PerformSelection(Random rnd, ISpecies species);

        /// <summary>
        /// Selects an unfit genome.
        /// </summary>
        /// <param name="rnd">A random number generator.</param>
        /// <param name="species">The species to select the genome from.</param>
        /// <returns>The selected genome.</returns>
        int PerformAntiSelection(Random rnd, ISpecies species);

        /// <summary>
        /// The trainer being used.
        /// </summary>
        IEvolutionaryAlgorithm Trainer { get; }
    }
}
