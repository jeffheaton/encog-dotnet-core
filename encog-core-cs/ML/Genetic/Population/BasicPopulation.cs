using System;
using System.Collections.Generic;
using Encog.ML.Genetic.Genome;
using Encog.ML.Genetic.Innovation;
using Encog.ML.Genetic.Species;
using Encog.Util.Identity;

namespace Encog.ML.Genetic.Population
{
    /// <summary>
    /// Defines the basic functionality for a population of genomes.
    /// </summary>
    [Serializable]
    public class BasicPopulation : IPopulation
    {
        /// <summary>
        /// Thed default old age penalty.
        /// </summary>
        ///
        public const double DEFAULT_OLD_AGE_PENALTY = 0.3d;

        /// <summary>
        /// The default old age threshold.
        /// </summary>
        ///
        public const int DEFAULT_OLD_AGE_THRESHOLD = 50;

        /// <summary>
        /// The default survival rate.
        /// </summary>
        ///
        public const double DEFAULT_SURVIVAL_RATE = 0.2d;

        /// <summary>
        /// The default youth penalty.
        /// </summary>
        ///
        public const double DEFAULT_YOUTH_BONUS = 0.3d;

        /// <summary>
        /// The default youth threshold.
        /// </summary>
        ///
        public const int DEFAULT_YOUTH_THRESHOLD = 10;

        /// <summary>
        /// Generate gene id's.
        /// </summary>
        ///
        private readonly IGenerateID geneIDGenerate;

        /// <summary>
        /// Generate genome id's.
        /// </summary>
        ///
        private readonly IGenerateID genomeIDGenerate;

        /// <summary>
        /// The population.
        /// </summary>
        ///
        private readonly List<IGenome> genomes;

        /// <summary>
        /// Generate innovation id's.
        /// </summary>
        ///
        private readonly IGenerateID innovationIDGenerate;
       
        /// <summary>
        /// Generate species id's.
        /// </summary>
        ///
        private readonly IGenerateID speciesIDGenerate;

        /// <summary>
        /// The young threshold.
        /// </summary>
        ///
        private int youngBonusAgeThreshold;

        /// <summary>
        /// Construct an empty population.
        /// </summary>
        ///
        public BasicPopulation()
        {
            geneIDGenerate = new BasicGenerateID();
            genomeIDGenerate = new BasicGenerateID();
            genomes = new List<IGenome>();
            innovationIDGenerate = new BasicGenerateID();
            OldAgePenalty = DEFAULT_OLD_AGE_PENALTY;
            OldAgeThreshold = DEFAULT_OLD_AGE_THRESHOLD;
            Species = new List<ISpecies>();
            speciesIDGenerate = new BasicGenerateID();
            SurvivalRate = DEFAULT_SURVIVAL_RATE;
            youngBonusAgeThreshold = DEFAULT_YOUTH_THRESHOLD;
            YoungScoreBonus = DEFAULT_YOUTH_BONUS;
            PopulationSize = 0;
        }

        /// <summary>
        /// Construct a population.
        /// </summary>
        /// <param name="thePopulationSize">The population size.</param>
        public BasicPopulation(int thePopulationSize)
        {
            geneIDGenerate = new BasicGenerateID();
            genomeIDGenerate = new BasicGenerateID();
            genomes = new List<IGenome>();
            innovationIDGenerate = new BasicGenerateID();
            OldAgePenalty = DEFAULT_OLD_AGE_PENALTY;
            OldAgeThreshold = DEFAULT_OLD_AGE_THRESHOLD;
            Species = new List<ISpecies>();
            speciesIDGenerate = new BasicGenerateID();
            SurvivalRate = DEFAULT_SURVIVAL_RATE;
            youngBonusAgeThreshold = DEFAULT_YOUTH_THRESHOLD;
            YoungScoreBonus = DEFAULT_YOUTH_BONUS;
            PopulationSize = thePopulationSize;
        }

        /// <value>the geneIDGenerate</value>
        public IGenerateID GeneIDGenerate
        {
            get { return geneIDGenerate; }
        }


        /// <value>the genomeIDGenerate</value>
        public IGenerateID GenomeIDGenerate
        {
            get { return genomeIDGenerate; }
        }

        /// <value>the innovationIDGenerate</value>
        public IGenerateID InnovationIDGenerate
        {
            get { return innovationIDGenerate; }
        }

        /// <summary>
        /// Set the name.
        /// </summary>
        public String Name { get; set; }

        /// <value>the speciesIDGenerate</value>
        public IGenerateID SpeciesIDGenerate
        {
            get { return speciesIDGenerate; }
        }

        #region IPopulation Members


        /// <inheritdoc/>
        public void Add(IGenome genome)
        {
            genomes.Add(genome);
            genome.Population = this;
        }

        /// <inheritdoc/>
        public long AssignGeneID()
        {
            return geneIDGenerate.Generate();
        }

        /// <inheritdoc/>
        public long AssignGenomeID()
        {
            return genomeIDGenerate.Generate();
        }

        /// <inheritdoc/>
        public long AssignInnovationID()
        {
            return innovationIDGenerate.Generate();
        }

        /// <inheritdoc/>
        public long AssignSpeciesID()
        {
            return speciesIDGenerate.Generate();
        }

        /// <inheritdoc/>
        public void Claim(GeneticAlgorithm ga)
        {
            foreach (IGenome genome  in  genomes)
            {
                genome.GA = ga;
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            genomes.Clear();
        }

        /// <inheritdoc/>
        public IGenome Get(int i)
        {
            return genomes[i];
        }

        /// <inheritdoc/>
        public IGenome Best
        {
            get
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
        }


        /// <inheritdoc/>
        public IList<IGenome> Genomes
        {
            get { return genomes; }
        }


        /// <inheritdoc/>
        public IInnovationList Innovations { get; set; }


        /// <inheritdoc/>
        public double OldAgePenalty { get; set; }


        /// <inheritdoc/>
        public int OldAgeThreshold { get; set; }


        /// <inheritdoc/>
        public int PopulationSize { get; set; }


        /// <inheritdoc/>
        public IList<ISpecies> Species { get; set; }


        /// <inheritdoc/>
        public double SurvivalRate { get; set; }


        /// <value>the youngBonusAgeThreshold to set</value>
        public int YoungBonusAgeThreshold
        {
            get { return youngBonusAgeThreshold; }
            set { youngBonusAgeThreshold = value; }
        }


        /// <inheritdoc/>
        public double YoungScoreBonus { get; set; }


        /// <inheritdoc/>
        public int YoungBonusAgeThreshhold
        {
            set { youngBonusAgeThreshold = value; }
        }


        /// <inheritdoc/>
        public int Size()
        {
            return genomes.Count;
        }

        /// <inheritdoc/>
        public void Sort()
        {
            genomes.Sort();
        }

        #endregion
    }
}