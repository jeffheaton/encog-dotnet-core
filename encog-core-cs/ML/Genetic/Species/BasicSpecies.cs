
 namespace Encog.ML.Genetic.Species {
	
	using Encog.ML.Genetic.Genome;
	using Encog.ML.Genetic.Population;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Runtime.Serialization;
	
	/// <summary>
	/// Provides basic functionality for a species.
	/// </summary>
	///
	[Serializable]
	public class BasicSpecies : ISpecies {
	
		/// <summary>
		/// Serial id.
		/// </summary>
		///
		private const long serialVersionUID = 1L;
	
		/// <summary>
		/// The age of this species.
		/// </summary>
		///
		private int age;
	
		/// <summary>
		/// The best score.
		/// </summary>
		///
		private double bestScore;
	
		/// <summary>
		/// The number of generations with no improvement.
		/// </summary>
		///
		private int gensNoImprovement;
	
		/// <summary>
		/// The leader.
		/// </summary>
		///
		private IGenome leader;
	
		/// <summary>
		/// The list of genomes.
		/// </summary>
		///
		private readonly IList<IGenome> members;
	
		/// <summary>
		/// The number of spawns required.
		/// </summary>
		///
		private double spawnsRequired;
	
		/// <summary>
		/// The species id.
		/// </summary>
		///
		private long speciesID;
	
		/// <summary>
		/// The owner class.
		/// </summary>
		///
		private IPopulation population;
	
		/// <summary>
		/// The id of the leader.
		/// </summary>
		///
		private long leaderID;
	
		/// <summary>
		/// Default constructor, used mainly for persistence.
		/// </summary>
		///
		public BasicSpecies() {
			this.members = new List<IGenome>();
	
		}
	
		/// <summary>
		/// Construct a species.
		/// </summary>
		///
		/// <param name="thePopulation">The population the species belongs to.</param>
		/// <param name="theFirst">The first genome in the species.</param>
		/// <param name="theSpeciesID">The species id.</param>
		public BasicSpecies(IPopulation thePopulation, IGenome theFirst,
				long theSpeciesID) {
			this.members = new List<IGenome>();
			this.population = thePopulation;
			this.speciesID = theSpeciesID;
			this.bestScore = theFirst.Score;
			this.gensNoImprovement = 0;
			this.age = 0;
			this.leader = theFirst;
			this.spawnsRequired = 0;
			this.members.Add(theFirst);
		}
	
		/// <summary>
		/// Calculate the amount to spawn.
		/// </summary>
		///
		public void CalculateSpawnAmount() {
			this.spawnsRequired = 0;
			/* foreach */
			foreach (IGenome genome  in  this.members) {
				this.spawnsRequired += genome.AmountToSpawn;
			}
	
		}
	
		/// <summary>
		/// Choose a parent to mate. Choose from the population, determined by the
		/// survival rate. From this pool, a random parent is chosen.
		/// </summary>
		///
		/// <returns>The parent.</returns>
		public IGenome ChooseParent() {
			IGenome baby;
	
			// If there is a single member, then choose that one.
			if (this.members.Count == 1) {
				baby = this.members[0];
			} else {
				// If there are many, then choose the population based on survival
				// rate
				// and select a random genome.
				int maxIndexSize = (int) (this.population.SurvivalRate * this.members.Count) + 1;
				int theOne = (int) Encog.MathUtil.Randomize.RangeRandomizer.Randomize(0, maxIndexSize);
				baby = this.members[theOne];
			}
	
			return baby;
		}
	
		/// <summary>
		/// Set the age of this species.
		/// </summary>
		///
		/// <value>The age of this species.</value>
		public int Age {
		
		/// <returns>The age of this species.</returns>
		  get {
				return this.age;
			}
		/// <summary>
		/// Set the age of this species.
		/// </summary>
		///
		/// <param name="theAge">The age of this species.</param>
		  set {
				this.age = value;
			}
		}
		
	
		/// <summary>
		/// Set the best score.
		/// </summary>
		///
		/// <value>The best score.</value>
		public double BestScore {
		
		/// <returns>The best score for this species.</returns>
		  get {
				return this.bestScore;
			}
		/// <summary>
		/// Set the best score.
		/// </summary>
		///
		/// <param name="theBestScore">The best score.</param>
		  set {
				this.bestScore = value;
			}
		}
		
	
		/// <summary>
		/// Set the number of generations with no improvement.
		/// </summary>
		///
		/// <value>The number of generations.</value>
		public int GensNoImprovement {
		
		/// <returns>The number of generations with no improvement.</returns>
		  get {
				return this.gensNoImprovement;
			}
		/// <summary>
		/// Set the number of generations with no improvement.
		/// </summary>
		///
		/// <param name="theGensNoImprovement">The number of generations.</param>
		  set {
				this.gensNoImprovement = value;
			}
		}
		
	
		/// <summary>
		/// Set the leader.
		/// </summary>
		///
		/// <value>The new leader.</value>
		public IGenome Leader {
		
		/// <returns>THe leader of this species.</returns>
		  get {
				return this.leader;
			}
		/// <summary>
		/// Set the leader.
		/// </summary>
		///
		/// <param name="theLeader">The new leader.</param>
		  set {
				this.leader = value;
			}
		}
		
	
		
		/// <value>The members of this species.</value>
		public IList<IGenome> Members {
		
		/// <returns>The members of this species.</returns>
		  get {
				return this.members;
			}
		}
		
	
		
		/// <value>The number to spawn.</value>
		public double NumToSpawn {
		
		/// <returns>The number to spawn.</returns>
		  get {
				return this.spawnsRequired;
			}
		}
		
	
		
		/// <value>the population to set</value>
		public IPopulation Population {
		
		/// <returns>The population that this species belongs to.</returns>
		  get {
				return this.population;
			}
		
		/// <param name="thePopulation">the population to set</param>
		  set {
				this.population = value;
			}
		}
		
	
		/// <summary>
		/// Set the number of spawns required.
		/// </summary>
		///
		/// <value>The number of spawns required.</value>
		public double SpawnsRequired {
		
		/// <returns>The spawns required.</returns>
		  get {
				return this.spawnsRequired;
			}
		/// <summary>
		/// Set the number of spawns required.
		/// </summary>
		///
		/// <param name="theSpawnsRequired">The number of spawns required.</param>
		  set {
				this.spawnsRequired = value;
			}
		}
		
		/// <summary>
		/// Set the leader id. This value is not persisted, it is used only for
		/// loading.
		/// </summary>
		///
		/// <value>the leaderID to set</value>
		public long TempLeaderID {
		
		/// <returns>the leaderID</returns>
		  get {
				return this.leaderID;
			}
		/// <summary>
		/// Set the leader id. This value is not persisted, it is used only for
		/// loading.
		/// </summary>
		///
		/// <param name="theLeaderID">the leaderID to set</param>
		  set {
				this.leaderID = value;
			}
		}
		
	
		/// <summary>
		/// Purge all members, increase age by one and count the number of
		/// generations with no improvement.
		/// </summary>
		///
		public void Purge() {
			this.members.Clear();
			this.age++;
			this.gensNoImprovement++;
			this.spawnsRequired = 0;
	
		}
	
		/// <summary>
		/// Set the species id.
		/// </summary>
		///
		/// <value>The new species id.</value>
		public long SpeciesID {
            get { return this.speciesID;  }
		/// <summary>
		/// Set the species id.
		/// </summary>
		///
		/// <param name="i">The new species id.</param>
		  set {
				this.speciesID = value;
			}
		}
		
	
	}
}
