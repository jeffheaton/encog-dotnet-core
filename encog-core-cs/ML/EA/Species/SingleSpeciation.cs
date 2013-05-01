using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Train;
using Encog.ML.EA.Sort;
using Encog.ML.EA.Genome;

namespace Encog.ML.EA.Species
{
    /// <summary>
    /// This speciation strategy simply creates a single species that contains the
    /// entire population. Use this speciation strategy if you do not wish to use
    /// species.
    /// </summary>
    [Serializable]
    public class SingleSpeciation : ISpeciation
    {
        /// <summary>
        /// The trainer.
        /// </summary>
        private IEvolutionaryAlgorithm owner;

        /// <summary>
        /// The method used to sort the genomes in the species. More desirable
        /// genomes should come first for later selection.
        /// </summary>
        private SortGenomesForSpecies sortGenomes;

        /// <inheritdoc/>
        public void Init(IEvolutionaryAlgorithm theOwner)
        {
            this.owner = theOwner;
            this.sortGenomes = new SortGenomesForSpecies(this.owner);
        }

        /// <inheritdoc/>
        public void PerformSpeciation(IList<IGenome> genomeList)
        {
            UpdateShare();
            ISpecies species = this.owner.Population.Species[0];
            species.Members.Clear();
            species.Members = species.Members.Union(genomeList).ToList();
            species.Members.Sort(this.sortGenomes);
            species.Leader = species.Members[0];

        }

        /// <inheritdoc/>
        private void UpdateShare()
        {
            int speciesCount = this.owner.Population.Species.Count;
            if (speciesCount != 1)
            {
                throw new EncogError(
                        "SingleSpeciation can only be used with a species count of 1, species count is "
                                + speciesCount);
            }

            ISpecies species = this.owner.Population.Species[0];
            species.OffspringCount = this.owner.Population.PopulationSize;
        }
    }
}
