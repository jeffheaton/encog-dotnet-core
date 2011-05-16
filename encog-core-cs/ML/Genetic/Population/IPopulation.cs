using System.Collections.Generic;
using Encog.ML.Genetic.Genome;
using Encog.ML.Genetic.Innovation;
using Encog.ML.Genetic.Species;

namespace Encog.ML.Genetic.Population
{
    /// <summary>
    /// Defines a population of genomes.
    /// </summary>
    ///
    public interface IPopulation
    {
        /// <value>The best genome in the population.</value>
        IGenome Best { /// <returns>The best genome in the population.</returns>
            get; }


        /// <value>The genomes in the population.</value>
        IList<IGenome> Genomes { /// <returns>The genomes in the population.</returns>
            get; }


        /// <summary>
        /// Set the innovations collection.
        /// </summary>
        ///
        /// <value>The innovations collection.</value>
        IInnovationList Innovations { /// <returns>A list of innovations in this population.</returns>
            get;
            /// <summary>
            /// Set the innovations collection.
            /// </summary>
            ///
            /// <param name="innovations">The innovations collection.</param>
            set; }


        /// <summary>
        /// Set the old age penalty.
        /// </summary>
        ///
        /// <value>The old age penalty.</value>
        double OldAgePenalty { /// <returns>The percent to decrease "old" genom's score by.</returns>
            get;
            /// <summary>
            /// Set the old age penalty.
            /// </summary>
            ///
            /// <param name="oldAgePenalty">The old age penalty.</param>
            set; }


        /// <summary>
        /// Set the age at which a genome is considered "old".
        /// </summary>
        ///
        /// <value>The old age threshold.</value>
        int OldAgeThreshold { /// <returns>The age at which to consider a genome "old".</returns>
            get;
            /// <summary>
            /// Set the age at which a genome is considered "old".
            /// </summary>
            ///
            /// <param name="oldAgeThreshold">The old age threshold.</param>
            set; }


        /// <summary>
        /// Set the max population size.
        /// </summary>
        ///
        /// <value>The max population size.</value>
        int PopulationSize { /// <returns>The max population size.</returns>
            get;
            /// <summary>
            /// Set the max population size.
            /// </summary>
            ///
            /// <param name="populationSize">The max population size.</param>
            set; }


        /// <value>A list of species.</value>
        IList<ISpecies> Species { /// <returns>A list of species.</returns>
            get; }


        /// <summary>
        /// Set the survival rate.
        /// </summary>
        ///
        /// <value>The survival rate.</value>
        double SurvivalRate { /// <returns>The survival rate.</returns>
            get;
            /// <summary>
            /// Set the survival rate.
            /// </summary>
            ///
            /// <param name="survivalRate">The survival rate.</param>
            set; }


        /// <value>The age, below which, a genome is considered "young".</value>
        int YoungBonusAgeThreshold { /// <returns>The age, below which, a genome is considered "young".</returns>
            get; }


        /// <summary>
        /// Set the youth score bonus.
        /// </summary>
        ///
        /// <value>The bonus.</value>
        double YoungScoreBonus { /// <returns>The bonus given to "young" genomes.</returns>
            get;
            /// <summary>
            /// Set the youth score bonus.
            /// </summary>
            ///
            /// <param name="youngScoreBonus">The bonus.</param>
            set; }


        /// <summary>
        /// Set the age at which genoms are considered young.
        /// </summary>
        ///
        /// <value>The age.</value>
        int YoungBonusAgeThreshhold { /// <summary>
            /// Set the age at which genoms are considered young.
            /// </summary>
            ///
            /// <param name="youngBonusAgeThreshhold">The age.</param>
            set; }

        /// <summary>
        /// Add a genome to the population.
        /// </summary>
        ///
        /// <param name="genome">The genome to add.</param>
        void Add(IGenome genome);

        /// <returns>Assign a gene id.</returns>
        long AssignGeneID();


        /// <returns>Assign a genome id.</returns>
        long AssignGenomeID();


        /// <returns>Assign an innovation id.</returns>
        long AssignInnovationID();


        /// <returns>Assign a species id.</returns>
        long AssignSpeciesID();

        /// <summary>
        /// Clear all genomes from this population.
        /// </summary>
        ///
        void Clear();

        /// <summary>
        /// Get a genome by index.  Index 0 is the best genome.
        /// </summary>
        ///
        /// <param name="i">The genome to get.</param>
        /// <returns>The genome at the specified index.</returns>
        IGenome Get(int i);


        /// <returns>The size of the population.</returns>
        int Size();

        /// <summary>
        /// Sort the population by best score.
        /// </summary>
        ///
        void Sort();

        /// <summary>
        /// Claim the population, before training.
        /// </summary>
        ///
        /// <param name="ga">The GA that is claiming.</param>
        void Claim(GeneticAlgorithm ga);
    }
}