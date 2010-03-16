using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;

namespace Encog.Solve.Genetic.Species
{
    /// <summary>
    /// A species is a set of related genomes.  Crossover occurs within a species.
    /// </summary>
    public interface ISpecies
    {
        /// <summary>
        /// Add a genome to this species.
        /// </summary>
        /// <param name="genome">The genome to add.</param>
        void AddMember(IGenome genome);

        /// <summary>
        /// Adjust the score of this species. This is where old age and youth
        /// bonus/penalties happen.
        /// </summary>
        void AdjustScore();

        /// <summary>
        /// Calculate the amount that a species will spawn.
        /// </summary>
        void CalculateSpawnAmount();

        /// <summary>
        /// Choose a worthy parent for mating.
        /// </summary>
        /// <returns>The parent genome.</returns>
        IGenome ChooseParent();

        /// <summary>
        /// The age of this species.
        /// </summary>
        int Age { get; }

        /// <summary>
        /// The best score for this species.
        /// </summary>
        double BestScore { get; }

        /// <summary>
        /// How many generations with no improvement.
        /// </summary>
        int GensNoImprovement { get; set; }

        /// <summary>
        /// Get the leader for this species. The leader is the genome with
        /// the best score.
        /// </summary>
        IGenome Leader { get; set; }

        /// <summary>
        /// The numbers of this species.
        /// </summary>
        IList<IGenome> Members { get; }

        /// <summary>
        /// The number of genomes this species will try to spawn into the
        /// next generation.
        /// </summary>
        double NumToSpawn { get; set; }

        /// <summary>
        /// The number of spawns this species requires.
        /// </summary>
        double SpawnsRequired { get; set; }

        /// <summary>
        /// The species ID.
        /// </summary>
        long SpeciesID { get; set; }

        /// <summary>
        /// Purge old unsuccessful genomes.
        /// </summary>
        void Purge();

    }
}
