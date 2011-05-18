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
        /// The species in this population.
        /// </summary>
        ///
        private readonly IList<ISpecies> species;

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
            species = new List<ISpecies>();
            speciesIDGenerate = new BasicGenerateID();
            SurvivalRate = DEFAULT_SURVIVAL_RATE;
            youngBonusAgeThreshold = DEFAULT_YOUTH_THRESHOLD;
            YoungScoreBonus = DEFAULT_YOUTH_BONUS;
            PopulationSize = 0;
        }

        /// <summary>
        /// Construct a population.
        /// </summary>
        ///
        /// <param name="thePopulationSize">The population size.</param>
        public BasicPopulation(int thePopulationSize)
        {
            geneIDGenerate = new BasicGenerateID();
            genomeIDGenerate = new BasicGenerateID();
            genomes = new List<IGenome>();
            innovationIDGenerate = new BasicGenerateID();
            OldAgePenalty = DEFAULT_OLD_AGE_PENALTY;
            OldAgeThreshold = DEFAULT_OLD_AGE_THRESHOLD;
            species = new List<ISpecies>();
            speciesIDGenerate = new BasicGenerateID();
            SurvivalRate = DEFAULT_SURVIVAL_RATE;
            youngBonusAgeThreshold = DEFAULT_YOUTH_THRESHOLD;
            YoungScoreBonus = DEFAULT_YOUTH_BONUS;
            PopulationSize = thePopulationSize;
        }

        /// <value>the geneIDGenerate</value>
        public IGenerateID GeneIDGenerate
        {
            /// <returns>the geneIDGenerate</returns>
            get { return geneIDGenerate; }
        }


        /// <value>the genomeIDGenerate</value>
        public IGenerateID GenomeIDGenerate
        {
            /// <returns>the genomeIDGenerate</returns>
            get { return genomeIDGenerate; }
        }

        /// <value>the innovationIDGenerate</value>
        public IGenerateID InnovationIDGenerate
        {
            /// <returns>the innovationIDGenerate</returns>
            get { return innovationIDGenerate; }
        }

        /// <summary>
        /// Set the name.
        /// </summary>
        ///
        /// <value>The new name.</value>
        public String Name { /// <returns>The name.</returns>
            get; /// <summary>
            /// Set the name.
            /// </summary>
            ///
            /// <param name="theName">The new name.</param>
            set; }

        /// <value>the speciesIDGenerate</value>
        public IGenerateID SpeciesIDGenerate
        {
            /// <returns>the speciesIDGenerate</returns>
            get { return speciesIDGenerate; }
        }

        #region IPopulation Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Add(IGenome genome)
        {
            genomes.Add(genome);
            genome.Population = this;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public long AssignGeneID()
        {
            return geneIDGenerate.Generate();
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public long AssignGenomeID()
        {
            return genomeIDGenerate.Generate();
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public long AssignInnovationID()
        {
            return innovationIDGenerate.Generate();
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public long AssignSpeciesID()
        {
            return speciesIDGenerate.Generate();
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Claim(GeneticAlgorithm ga)
        {
            foreach (IGenome genome  in  genomes)
            {
                genome.GA = ga;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Clear()
        {
            genomes.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public IGenome Get(int i)
        {
            return genomes[i];
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public IGenome Best
        {
            /// <summary>
            /// 
            /// </summary>
            ///
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


        /// <summary>
        /// 
        /// </summary>
        ///
        public IList<IGenome> Genomes
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return genomes; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public IInnovationList Innovations { /// <summary>
            /// 
            /// </summary>
            ///
            get; /// <summary>
            /// 
            /// </summary>
            ///
            set; }


        /// <summary>
        /// 
        /// </summary>
        ///
        public double OldAgePenalty { /// <summary>
            /// 
            /// </summary>
            ///
            get; /// <summary>
            /// 
            /// </summary>
            ///
            set; }


        /// <summary>
        /// 
        /// </summary>
        ///
        public int OldAgeThreshold { /// <summary>
            /// 
            /// </summary>
            ///
            get; /// <summary>
            /// 
            /// </summary>
            ///
            set; }


        /// <summary>
        /// 
        /// </summary>
        ///
        public int PopulationSize { /// <summary>
            /// 
            /// </summary>
            ///
            get; /// <summary>
            /// 
            /// </summary>
            ///
            set; }


        /// <summary>
        /// 
        /// </summary>
        ///
        public IList<ISpecies> Species
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return species; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public double SurvivalRate { /// <summary>
            /// 
            /// </summary>
            ///
            get; /// <summary>
            /// 
            /// </summary>
            ///
            set; }


        /// <value>the youngBonusAgeThreshold to set</value>
        public int YoungBonusAgeThreshold
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return youngBonusAgeThreshold; }
            /// <param name="theYoungBonusAgeThreshold">the youngBonusAgeThreshold to set</param>
            set { youngBonusAgeThreshold = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public double YoungScoreBonus { /// <summary>
            /// 
            /// </summary>
            ///
            get; /// <summary>
            /// 
            /// </summary>
            ///
            set; }


        /// <summary>
        /// 
        /// </summary>
        ///
        public int YoungBonusAgeThreshhold
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            set { youngBonusAgeThreshold = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public int Size()
        {
            return genomes.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void Sort()
        {
            genomes.Sort();
        }

        #endregion
    }
}