 namespace Encog.ML.Genetic {
	
	using Encog.ML.Genetic.Genome;
	using Encog.Util.Concurrency;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// Provides a basic implementation of a genetic algorithm.
	/// </summary>
	///
	public class BasicGeneticAlgorithm : GeneticAlgorithm {
	
		public BasicGeneticAlgorithm() {
			this.first = true;
		}
	
		/// <summary>
		/// Is this the first iteration.
		/// </summary>
		///
		private bool first;
	
		/// <summary>
		/// Modify the weight matrix and bias values based on the last call to
		/// calcError.
		/// </summary>
		///
		/// <exception cref="NeuralNetworkException"></exception>
		public sealed override void Iteration() {
	
			if (this.first) {
				Population.Claim(this);
				this.first = false;
			}
	
			int countToMate = (int) (Population.PopulationSize * PercentToMate);
			int offspringCount = countToMate * 2;
			int offspringIndex = Population.PopulationSize
					- offspringCount;
			int matingPopulationSize = (int) (Population.PopulationSize * MatingPopulation);
	
			TaskGroup group = Encog.Util.Concurrency.EngineConcurrency.Instance
					.CreateTaskGroup();
	
			// mate and form the next generation
			for (int i = 0; i < countToMate; i++) {
				IGenome mother = Population.Genomes[i];
				int fatherInt = (int) ((new Random()).Next() * matingPopulationSize);
				IGenome father = Population.Genomes[fatherInt];
				IGenome child1 = Population.Genomes[offspringIndex];
				IGenome child2 = Population.Genomes[offspringIndex + 1];
	
				MateWorker worker = new MateWorker(mother, father, child1,
						child2);
	
				Encog.Util.Concurrency.EngineConcurrency.Instance.ProcessTask(worker, group);
	
				offspringIndex += 2;
			}
	
			group.WaitForComplete();
	
			// sort the next generation
			Population.Sort();
		}
	
	}
}
