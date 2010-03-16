using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.Concurrency;
using Encog.Solve.Genetic.Genome;
using Encog.Solve.Genetic.Crossover;
using Encog.Solve.Genetic.Mutate;
using Encog.Solve.Genetic.Population;
using Encog.MathUtil;

namespace Encog.Solve.Genetic
{
    public class GeneticAlgorithm
    {
        /**
	 * Threadpool timeout.
	 */
	public const int TIMEOUT = 120;

	private ICalculateGenomeScore calculateScore;

	public GenomeComparator Comparator { get; set; }

	public ICrossover Crossover { get; set; }

	/**
	 * Percent of the population that the mating population chooses partners.
	 * from.
	 */
	private double MatingPopulation { get; set; }

	private IMutate mutate;

	/**
	 * The percent that should mutate.
	 */
	private double MutationPercent { get; set; }

	/**
	 * What percent should be chosen to mate. They will choose partners from the
	 * entire mating population.
	 */
    private double percentToMate { get; set; }

	private IPopulation Population { get; set; }

	public void CalculateScore(IGenome g) {
		double score = calculateScore.CalculateScore(g);
		g.setScore(score);
	}

	public IMutate getMutate() {
		return mutate;
	}

	/**
	 * Get the percent to mate.
	 * 
	 * @return The percent to mate.
	 */
	public double getPercentToMate() {
		return percentToMate;
	}

	/**
	 * Modify the weight matrix and thresholds based on the last call to
	 * calcError.
	 * 
	 * @throws NeuralNetworkException
	 */
	public void iteration() {

		int countToMate = (int) (Population.getPopulationSize() * getPercentToMate());
		int offspringCount = countToMate * 2;
		int offspringIndex = Population.getPopulationSize() - offspringCount;
		int matingPopulationSize = (int) (Population.getPopulationSize() * MatingPopulation );

		TaskGroup group = EncogConcurrency.Instance
				.CreateTaskGroup();

		// mate and form the next generation
		for (int i = 0; i < countToMate; i++) {
			IGenome mother = Population.getGenomes()[i];
			int fatherInt = (int) (ThreadSafeRandom.NextDouble() * matingPopulationSize);
			IGenome father = Population.getGenomes()[fatherInt];
			IGenome child1 = Population.getGenomes()[offspringIndex];
			IGenome child2 = Population.getGenomes()[
					offspringIndex + 1];

			MateWorker worker = new MateWorker(mother, father, child1,
					child2);

			EncogConcurrency.Instance.ProcessTask(worker);

			offspringIndex += 2;
		}

		group.WaitForComplete();

		// sort the next generation
		Population.sort();
	}

	public void setCalculateScore( ICalculateGenomeScore calculateScore) {
		this.calculateScore = calculateScore;
	}


    }
}
