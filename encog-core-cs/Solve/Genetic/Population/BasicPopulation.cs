using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Identity;
using Encog.Solve.Genetic.Genome;
using Encog.Solve.Genetic.Innovation;
using Encog.Solve.Genetic.Species;

namespace Encog.Solve.Genetic.Population
{
    public class BasicPopulation : IPopulation
    {
        /// <summary>
        /// A list of innovations in this population.
        /// </summary>
        public IInnovationList Innovations { get; set; }

        /// <summary>
        /// The percent to decrease "old" genom's score by.
        /// </summary>
        public double OldAgePenalty { get; set; }

        /// <summary>
        /// The age at which to consider a genome "old".
        /// </summary>
        public int OldAgeThreshold { get; set; }

        /// <summary>
        /// The max population size.
        /// </summary>
        public int PopulationSize { get; set; }

        /// <summary>
        /// The survival rate.
        /// </summary>
        public double SurvivalRate { get; set; }

        /// <summary>
        /// The age, below which, a genome is considered "young".
        /// </summary>
        public int YoungBonusAgeThreshold { get; set; }

        /// <summary>
        /// The bonus given to "young" genomes.
        /// </summary>
        public double YoungScoreBonus { get; set; }

        /// <summary>
        /// Generate gene id's.
        /// </summary>
        private IGenerateID geneIDGenerate = new BasicGenerateID();

        /// <summary>
        /// Generate genome id's.
        /// </summary>
        private IGenerateID genomeIDGenerate = new BasicGenerateID();

        /// <summary>
        /// The population.
        /// </summary>
        private List<IGenome> genomes = new List<IGenome>();

        /// <summary>
        /// Generate innovation id's.
        /// </summary>
        private IGenerateID innovationIDGenerate = new BasicGenerateID();

        
        /// <summary>
        /// How many genomes should be created.
        /// </summary>
        private int populationSize;

        /// <summary>
        /// The species in this population.
        /// </summary>
        private IList<ISpecies> species = new List<ISpecies>();

        /// <summary>
        /// Generate species id's.
        /// </summary>
        private IGenerateID speciesIDGenerate = new BasicGenerateID();


        /// <summary>
        /// Construct a population.
        /// </summary>
        /// <param name="populationSize">The population size.</param>
        public BasicPopulation(int populationSize)
        {
            this.populationSize = populationSize;
        }

        /// <summary>
        /// Add a genome to the population.
        /// </summary>
        /// <param name="genome">The genome to add.</param>
        public void Add(IGenome genome)
        {
            genomes.Add(genome);

        }

        /// <summary>
        /// Add all of the specified members to this population. 
        /// </summary>
        /// <param name="newPop">A list of new genomes to add.</param>
        public void AddAll(IList<IGenome> newPop)
        {
            genomes.Clear();
            foreach (IGenome g in newPop)
            {
                this.genomes.Add(g);
            }
        }


        /// <summary>
        /// Assign a gene id.
        /// </summary>
        /// <returns>The gene id.</returns>
        public long AssignGeneID()
        {
            return geneIDGenerate.Generate();
        }

        /// <summary>
        /// Assign a genome id.
        /// </summary>
        /// <returns>The genome id.</returns>
        public long AssignGenomeID()
        {
            return genomeIDGenerate.Generate();
        }

        /// <summary>
        /// Assign an innovation id.
        /// </summary>
        /// <returns>The innovation id.</returns>
        public long AssignInnovationID()
        {
            return innovationIDGenerate.Generate();
        }

        /// <summary>
        /// Assign a species id.
        /// </summary>
        /// <returns>The species id.</returns>
        public long AssignSpeciesID()
        {
            return speciesIDGenerate.Generate();
        }

        /// <summary>
        /// Clear all genomes from this population.
        /// </summary>
        public void Clear()
        {
            genomes.Clear();
        }

        /// <summary>
        /// The best genome in the population.
        /// </summary>
        /// <returns>The genome.</returns>
        public IGenome GetBest()
        {
            if (genomes.Count == 0)
            {
                return null;
            }
            else
            {
                return genomes[0];
            }
        }

        /// <summary>
        /// The genomes in the population.
        /// </summary>
        public IList<IGenome> Genomes
        {
            get
            {
                return genomes;
            }
        }

        /// <summary>
        /// The species in this population.
        /// </summary>
        public IList<ISpecies> Species
        {
            get
            {
                return this.species;
            }
        }

        /// <summary>
        /// Sort the population.
        /// </summary>
        public void Sort()
        {
            genomes.Sort();
        }

    }
}

