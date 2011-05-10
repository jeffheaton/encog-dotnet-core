
 namespace Encog.ML.Genetic.Population {
	
	using Encog.ML.Genetic;
	using Encog.ML.Genetic.Genome;
	using Encog.ML.Genetic.Innovation;
	using Encog.ML.Genetic.Species;
	using Encog.Util.Identity;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// Defines the basic functionality for a population of genomes.
	/// </summary>
	///
	public class BasicPopulation : IPopulation {
	
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
		/// The serial id.
		/// </summary>
		///
		private const long serialVersionUID = 1L;
	
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
		/// A list of innovations, or null if this feature is not being used.
		/// </summary>
		///
		private IInnovationList innovations;
	
		/// <summary>
		/// The old age penalty.
		/// </summary>
		///
		private double oldAgePenalty;
	
		/// <summary>
		/// The old age threshold.
		/// </summary>
		///
		private int oldAgeThreshold;
	
		/// <summary>
		/// How many genomes should be created.
		/// </summary>
		///
		private int populationSize;
	
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
		/// The survival rate.
		/// </summary>
		///
		private double survivalRate;
	
		/// <summary>
		/// The young threshold.
		/// </summary>
		///
		private int youngBonusAgeThreshold;
	
		/// <summary>
		/// The young score bonus.
		/// </summary>
		///
		private double youngScoreBonus;
	
		/// <summary>
		/// The object name.
		/// </summary>
		///
		private String name;
	
		/// <summary>
		/// Construct an empty population.
		/// </summary>
		///
		public BasicPopulation() {
			this.geneIDGenerate = new BasicGenerateID();
			this.genomeIDGenerate = new BasicGenerateID();
			this.genomes = new List<IGenome>();
			this.innovationIDGenerate = new BasicGenerateID();
			this.oldAgePenalty = DEFAULT_OLD_AGE_PENALTY;
			this.oldAgeThreshold = DEFAULT_OLD_AGE_THRESHOLD;
			this.species = new List<ISpecies>();
			this.speciesIDGenerate = new BasicGenerateID();
			this.survivalRate = DEFAULT_SURVIVAL_RATE;
			this.youngBonusAgeThreshold = DEFAULT_YOUTH_THRESHOLD;
			this.youngScoreBonus = DEFAULT_YOUTH_BONUS;
			this.populationSize = 0;
		}
	
		/// <summary>
		/// Construct a population.
		/// </summary>
		///
		/// <param name="thePopulationSize">The population size.</param>
		public BasicPopulation(int thePopulationSize) {
			this.geneIDGenerate = new BasicGenerateID();
			this.genomeIDGenerate = new BasicGenerateID();
			this.genomes = new List<IGenome>();
			this.innovationIDGenerate = new BasicGenerateID();
			this.oldAgePenalty = DEFAULT_OLD_AGE_PENALTY;
			this.oldAgeThreshold = DEFAULT_OLD_AGE_THRESHOLD;
			this.species = new List<ISpecies>();
			this.speciesIDGenerate = new BasicGenerateID();
			this.survivalRate = DEFAULT_SURVIVAL_RATE;
			this.youngBonusAgeThreshold = DEFAULT_YOUTH_THRESHOLD;
			this.youngScoreBonus = DEFAULT_YOUTH_BONUS;
			this.populationSize = thePopulationSize;
		}
	
		/// <summary>
		/// 
		/// </summary>
		///
		public void Add(IGenome genome) {
			this.genomes.Add(genome);
			genome.Population = this;
		}
	
		/// <summary>
		/// 
		/// </summary>
		///
		public long AssignGeneID() {
			return this.geneIDGenerate.Generate();
		}
	
		/// <summary>
		/// 
		/// </summary>
		///
		public long AssignGenomeID() {
			return this.genomeIDGenerate.Generate();
		}
	
		/// <summary>
		/// 
		/// </summary>
		///
		public long AssignInnovationID() {
			return this.innovationIDGenerate.Generate();
		}
	
		/// <summary>
		/// 
		/// </summary>
		///
		public long AssignSpeciesID() {
			return this.speciesIDGenerate.Generate();
		}
	
		/// <summary>
		/// 
		/// </summary>
		///
		public void Claim(GeneticAlgorithm ga) {
			/* foreach */
			foreach (IGenome genome  in  this.genomes) {
				genome.GeneticAlgorithm = ga;
			}
	
		}
	
		/// <summary>
		/// 
		/// </summary>
		///
		public void Clear() {
			this.genomes.Clear();
	
		}
	
		/// <summary>
		/// 
		/// </summary>
		///
		public IGenome Get(int i) {
			return this.genomes[i];
		}
	
		/// <summary>
		/// 
		/// </summary>
		///
		public IGenome Best {
		/// <summary>
		/// 
		/// </summary>
		///
		  get {
				if (this.genomes.Count == 0) {
					return null;
				} else {
					return this.genomes[0];
				}
			}
		}
		
	
		
		/// <value>the geneIDGenerate</value>
		public IGenerateID GeneIDGenerate {
		
		/// <returns>the geneIDGenerate</returns>
		  get {
				return this.geneIDGenerate;
			}
		}
		
	
		
		/// <value>the genomeIDGenerate</value>
		public IGenerateID GenomeIDGenerate {
		
		/// <returns>the genomeIDGenerate</returns>
		  get {
				return this.genomeIDGenerate;
			}
		}
		
	
		/// <summary>
		/// 
		/// </summary>
		///
		public IList<IGenome> Genomes {
		/// <summary>
		/// 
		/// </summary>
		///
		  get {
				return this.genomes;
			}
		}
		
	
		
		/// <value>the innovationIDGenerate</value>
		public IGenerateID InnovationIDGenerate {
		
		/// <returns>the innovationIDGenerate</returns>
		  get {
				return this.innovationIDGenerate;
			}
		}
		
	
		/// <summary>
		/// 
		/// </summary>
		///
		public IInnovationList Innovations {
		/// <summary>
		/// 
		/// </summary>
		///
		  get {
				return this.innovations;
			}
		/// <summary>
		/// 
		/// </summary>
		///
		  set {
				this.innovations = value;
			}
		}
		
	
		/// <summary>
		/// Set the name.
		/// </summary>
		///
		/// <value>The new name.</value>
		public String Name {
		
		/// <returns>The name.</returns>
		  get {
				return this.name;
			}
		/// <summary>
		/// Set the name.
		/// </summary>
		///
		/// <param name="theName">The new name.</param>
		  set {
				this.name = value;
		
			}
		}
		
	
		/// <summary>
		/// 
		/// </summary>
		///
		public double OldAgePenalty {
		/// <summary>
		/// 
		/// </summary>
		///
		  get {
				return this.oldAgePenalty;
			}
		/// <summary>
		/// 
		/// </summary>
		///
		  set {
				this.oldAgePenalty = value;
			}
		}
		
	
		/// <summary>
		/// 
		/// </summary>
		///
		public int OldAgeThreshold {
		/// <summary>
		/// 
		/// </summary>
		///
		  get {
				return this.oldAgeThreshold;
			}
		/// <summary>
		/// 
		/// </summary>
		///
		  set {
				this.oldAgeThreshold = value;
			}
		}
		
	
		/// <summary>
		/// 
		/// </summary>
		///
		public int PopulationSize {
		/// <summary>
		/// 
		/// </summary>
		///
		  get {
				return this.populationSize;
			}
		/// <summary>
		/// 
		/// </summary>
		///
		  set {
				this.populationSize = value;
			}
		}
		
	
		/// <summary>
		/// 
		/// </summary>
		///
		public IList<ISpecies> Species {
		/// <summary>
		/// 
		/// </summary>
		///
		  get {
				return this.species;
			}
		}
		
	
		
		/// <value>the speciesIDGenerate</value>
		public IGenerateID SpeciesIDGenerate {
		
		/// <returns>the speciesIDGenerate</returns>
		  get {
				return this.speciesIDGenerate;
			}
		}
		
	
		/// <summary>
		/// 
		/// </summary>
		///
		public double SurvivalRate {
		/// <summary>
		/// 
		/// </summary>
		///
		  get {
				return this.survivalRate;
			}
		/// <summary>
		/// 
		/// </summary>
		///
		  set {
				this.survivalRate = value;
			}
		}
		
	
		
		/// <value>the youngBonusAgeThreshold to set</value>
		public int YoungBonusAgeThreshold {
		/// <summary>
		/// 
		/// </summary>
		///
		  get {
				return this.youngBonusAgeThreshold;
			}
		
		/// <param name="theYoungBonusAgeThreshold">the youngBonusAgeThreshold to set</param>
		  set {
				this.youngBonusAgeThreshold = value;
			}
		}
		
	
		/// <summary>
		/// 
		/// </summary>
		///
		public double YoungScoreBonus {
		/// <summary>
		/// 
		/// </summary>
		///
		  get {
				return this.youngScoreBonus;
			}
		/// <summary>
		/// 
		/// </summary>
		///
		  set {
				this.youngScoreBonus = value;
			}
		}
		
	
		/// <summary>
		/// 
		/// </summary>
		///
		public int YoungBonusAgeThreshhold {
		/// <summary>
		/// 
		/// </summary>
		///
		  set {
				this.youngBonusAgeThreshold = value;
			}
		}
		
	
		/// <summary>
		/// 
		/// </summary>
		///
		public int Size() {
			return this.genomes.Count;
		}
	
		/// <summary>
		/// 
		/// </summary>
		///
		public void Sort()
		{
		    this.genomes.Sort();
		}
	}
}
