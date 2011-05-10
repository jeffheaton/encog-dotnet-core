 namespace Encog.ML.Genetic {
	
	using Encog.ML;
	using Encog.ML.Genetic.Crossover;
	using Encog.ML.Genetic.Genome;
	using Encog.ML.Genetic.Mutate;
	using Encog.ML.Genetic.Population;
	using Encog.ML.Genetic.Species;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
     using Encog.ML.Genetic.Genome;
	
	/// <summary>
	/// Implements a genetic algorithm. This is an abstract class. Other classes are
	/// provided by Encog use this base class to train neural networks or provide an
	/// answer to the traveling salesman problem.
	/// The genetic algorithm is also capable of using a thread pool to speed
	/// execution.
	/// </summary>
	///
	public abstract class GeneticAlgorithm {
	
		/// <summary>
		/// The score calculation object.
		/// </summary>
		///
		private CalculateGenomeScore calculateScore;
	
		/// <summary>
		/// The genome comparator.
		/// </summary>
		///
		private GenomeComparator comparator;
	
		/// <summary>
		/// The crossover object.
		/// </summary>
		///
		private ICrossover crossover;
	
		/// <summary>
		/// Percent of the population that the mating population chooses partners.
		/// from.
		/// </summary>
		///
		private double matingPopulation;
	
		/// <summary>
		/// The mutation object to use.
		/// </summary>
		///
		private IMutate mutate;
	
		/// <summary>
		/// The percent that should mutate.
		/// </summary>
		///
		private double mutationPercent;
	
		/// <summary>
		/// What percent should be chosen to mate. They will choose partners from the
		/// entire mating population.
		/// </summary>
		///
		private double percentToMate;
	
		/// <summary>
		/// The population.
		/// </summary>
		///
		private IPopulation population;
	
		/// <summary>
		/// Add a genome.
		/// </summary>
		///
		/// <param name="species">The species to add.</param>
		/// <param name="genome">The genome to add.</param>
		public void AddSpeciesMember(ISpecies species,
				IGenome genome) {
	
			if (Comparator.IsBetterThan(genome.Score,
					species.BestScore)) {
				species.BestScore = genome.Score;
				species.GensNoImprovement = 0;
				species.Leader = genome;
			}
	
			species.Members.Add(genome);
	
		}
	
		/// <summary>
		/// Calculate the score for this genome. The genome's score will be set.
		/// </summary>
		///
		/// <param name="g">The genome to calculate for.</param>
		public void PerformCalculateScore(IGenome g) {
			if (g.Organism   is  MLContext) {
				((MLContext) g.Organism).ClearContext();
			}
			double score = this.calculateScore.CalculateScore(g);
			g.Score = score;
		}
	
		/// <summary>
		/// Set the score calculation object.
		/// </summary>
		///
		/// <value>The score calculation object.</value>
		public CalculateGenomeScore CalculateScore {
		
		/// <returns>The score calculation object.</returns>
		  get {
				return this.calculateScore;
			}
		/// <summary>
		/// Set the score calculation object.
		/// </summary>
		///
		/// <param name="theCalculateScore">The score calculation object.</param>
		  set {
				this.calculateScore = value;
			}
		}
		
	
		/// <summary>
		/// Set the comparator.
		/// </summary>
		///
		/// <value>The comparator.</value>
		public GenomeComparator Comparator {
		
		/// <returns>The comparator.</returns>
		  get {
				return this.comparator;
			}
		/// <summary>
		/// Set the comparator.
		/// </summary>
		///
		/// <param name="theComparator">The comparator.</param>
		  set {
				this.comparator = value;
			}
		}
		
	
		/// <summary>
		/// Set the crossover object.
		/// </summary>
		///
		/// <value>The crossover object.</value>
		public ICrossover Crossover {
		
		/// <returns>The crossover object.</returns>
		  get {
				return this.crossover;
			}
		/// <summary>
		/// Set the crossover object.
		/// </summary>
		///
		/// <param name="theCrossover">The crossover object.</param>
		  set {
				this.crossover = value;
			}
		}
		
	
		/// <summary>
		/// Set the mating population percent.
		/// </summary>
		///
		/// <value>The mating population percent.</value>
		public double MatingPopulation {
		/// <summary>
		/// Get the mating population.
		/// </summary>
		///
		/// <returns>The mating population percent.</returns>
		  get {
				return this.matingPopulation;
			}
		/// <summary>
		/// Set the mating population percent.
		/// </summary>
		///
		/// <param name="theMatingPopulation">The mating population percent.</param>
		  set {
				this.matingPopulation = value;
			}
		}
		
	
		/// <summary>
		/// Set the mutate object.
		/// </summary>
		///
		/// <value>The mutate object.</value>
		public IMutate Mutate {
		
		/// <returns>The mutate object.</returns>
		  get {
				return this.mutate;
			}
		/// <summary>
		/// Set the mutate object.
		/// </summary>
		///
		/// <param name="theMutate">The mutate object.</param>
		  set {
				this.mutate = value;
			}
		}
		
	
		/// <summary>
		/// Set the mutation percent.
		/// </summary>
		///
		/// <value>The percent to mutate.</value>
		public double MutationPercent {
		/// <summary>
		/// Get the mutation percent.
		/// </summary>
		///
		/// <returns>The mutation percent.</returns>
		  get {
				return this.mutationPercent;
			}
		/// <summary>
		/// Set the mutation percent.
		/// </summary>
		///
		/// <param name="theMutationPercent">The percent to mutate.</param>
		  set {
				this.mutationPercent = value;
			}
		}
		
	
		/// <summary>
		/// Set the percent to mate.
		/// </summary>
		///
		/// <value>The percent to mate.</value>
		public double PercentToMate {
		/// <summary>
		/// Get the percent to mate.
		/// </summary>
		///
		/// <returns>The percent to mate.</returns>
		  get {
				return this.percentToMate;
			}
		/// <summary>
		/// Set the percent to mate.
		/// </summary>
		///
		/// <param name="thePercentToMate">The percent to mate.</param>
		  set {
				this.percentToMate = value;
			}
		}
		
	
		/// <summary>
		/// Set the population.
		/// </summary>
		///
		/// <value>The population.</value>
		public IPopulation Population {
		
		/// <returns>The population.</returns>
		  get {
				return this.population;
			}
		/// <summary>
		/// Set the population.
		/// </summary>
		///
		/// <param name="thePopulation">The population.</param>
		  set {
				this.population = value;
			}
		}
		
	
		/// <summary>
		/// Perform one training iteration.
		/// </summary>
		///
		public abstract void Iteration();
	}
}
