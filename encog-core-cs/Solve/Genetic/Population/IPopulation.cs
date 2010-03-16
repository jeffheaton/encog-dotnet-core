using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;
using Encog.Solve.Genetic.Innovation;
using Encog.Solve.Genetic.Species;

namespace Encog.Solve.Genetic.Population
{
    /// <summary>
    /// A population of genomes.
    /// </summary>
    public interface IPopulation
    {
        /// <summary>
        /// Add a genome to the population.
        /// </summary>
        /// <param name="genome">The genome to add.</param>
        void Add(IGenome genome);

        /// <summary>
        /// Add all of the specified members to this population.
        /// </summary>
        /// <param name="newPop">A list of new genomes to add.</param>
        void AddAll(IList<IGenome> newPop);

        /// <summary>
        /// Assign a gene id.
        /// </summary>
        /// <returns>The gene id.</returns>
        long AssignGeneID();

        /// <summary>
        /// Assign a genome id.
        /// </summary>
        /// <returns>The genome id.</returns>
        long AssignGenomeID();

        /// <summary>
        /// Assign an innovation id.
        /// </summary>
        /// <returns>The innovation id.</returns>
        long AssignInnovationID();

        /// <summary>
        /// Assign a species id.
        /// </summary>
        /// <returns></returns>
        long AssignSpeciesID();

        /// <summary>
        /// Clear all genomes from this population.
        /// </summary>
        void Clear();

        /// <summary>
        /// The best genome in the population.
        /// </summary>
        /// <returns></returns>
        IGenome GetBest();

        /// <summary>
        /// The genomes in the population.
        /// </summary>
        IList<IGenome> Genomes { get; }

        /// <summary>
        /// A list of innovations in this population.
        /// </summary>
        IInnovationList Innovations { get; set; }

        /// <summary>
        /// The percent to decrease "old" genom's score by.
        /// </summary>
        double OldAgePenalty { get; set; }

        /// <summary>
        /// The age at which to consider a genome "old".
        /// </summary>
        int OldAgeThreshold { get; set; }

        /// <summary>
        /// The max population size.
        /// </summary>
        int PopulationSize { get; set; }

        /// <summary>
        /// A list of species.
        /// </summary>
        IList<ISpecies> Species { get; }

        /// <summary>
        /// The survival rate.
        /// </summary>
        double SurvivalRate { get; set; }

        /// <summary>
        /// The age, below which, a genome is considered "young".
        /// </summary>
        int YoungBonusAgeThreshold { get; set; }

        /// <summary>
        /// The bonus given to "young" genomes.
        /// </summary>
        double YoungScoreBonus { get; set; }

        /// <summary>
        /// Sort the population by best score.
        /// </summary>
        void Sort();

    }
}
