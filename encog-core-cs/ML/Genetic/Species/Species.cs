using System.Collections.Generic;
using Encog.ML.Genetic.Genome;

namespace Encog.ML.Genetic.Species
{
    /// <summary>
    /// Defines the features used in a species. A species is a group of genomes.
    /// </summary>
    ///
    public interface ISpecies
    {
        /// <summary>
        /// Set the age of this species.
        /// </summary>
        ///
        /// <value>The age.</value>
        int Age { /// <returns>The age of this species.</returns>
            get;
            /// <summary>
            /// Set the age of this species.
            /// </summary>
            ///
            /// <param name="age">The age.</param>
            set; }


        /// <summary>
        /// Set the best score.
        /// </summary>
        ///
        /// <value>The best score.</value>
        double BestScore { /// <returns>The best score for this species.</returns>
            get;
            /// <summary>
            /// Set the best score.
            /// </summary>
            ///
            /// <param name="bestScore">The best score.</param>
            set; }


        /// <summary>
        /// Set the number of generations with no improvement.
        /// </summary>
        ///
        /// <value></value>
        int GensNoImprovement { /// <returns>How many generations with no improvement.</returns>
            get;
            /// <summary>
            /// Set the number of generations with no improvement.
            /// </summary>
            ///
            /// <param name="gensNoImprovement"></param>
            set; }


        /// <summary>
        /// Set the leader of this species.
        /// </summary>
        ///
        /// <value>The leader of this species.</value>
        IGenome Leader { /// <returns>Get the leader for this species. The leader is the genome with
            /// the best score.</returns>
            get;
            /// <summary>
            /// Set the leader of this species.
            /// </summary>
            ///
            /// <param name="leader">The leader of this species.</param>
            set; }


        /// <value>The numbers of this species.</value>
        IList<IGenome> Members { /// <returns>The numbers of this species.</returns>
            get; }


        /// <value>The number of genomes this species will try to spawn into the
        /// next generation.</value>
        double NumToSpawn { /// <returns>The number of genomes this species will try to spawn into the
            /// next generation.</returns>
            get; }


        /// <summary>
        /// Set the number of spawns required.
        /// </summary>
        ///
        /// <value>The number of spawns required.</value>
        double SpawnsRequired { /// <returns>The number of spawns this species requires.</returns>
            get;
            /// <summary>
            /// Set the number of spawns required.
            /// </summary>
            ///
            /// <param name="spawnsRequired">The number of spawns required.</param>
            set; }


        /// <value>The species ID.</value>
        long SpeciesID { /// <returns>The species ID.</returns>
            get; }

        /// <summary>
        /// Calculate the amount that a species will spawn.
        /// </summary>
        ///
        void CalculateSpawnAmount();

        /// <summary>
        /// Choose a worthy parent for mating.
        /// </summary>
        ///
        /// <returns>The parent genome.</returns>
        IGenome ChooseParent();


        /// <summary>
        /// Purge old unsuccessful genomes.
        /// </summary>
        ///
        void Purge();
    }
}