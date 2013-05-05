using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Species;
using Encog.ML.EA.Train;

namespace Encog.ML.EA.Sort
{
    /// <summary>
    /// This comparator is used to compare two species. This is done by comparing the
    /// scores of the two leaders.
    /// </summary>
    public class SpeciesComparer : Comparer<ISpecies>
    {
        /// <summary>
        /// The training method.
        /// </summary>
        private IEvolutionaryAlgorithm training;

        /// <summary>
        /// Create a species comparator.
        /// </summary>
        /// <param name="theTraining">The trainer.</param>
        public SpeciesComparer(IEvolutionaryAlgorithm theTraining)
        {
            this.training = theTraining;
        }

        /// <inheritdoc/>
        public override int Compare(ISpecies sp1, ISpecies sp2)
        {
            return training.BestComparer.Compare(sp1.Leader,
                    sp2.Leader);
        }
    }
}
