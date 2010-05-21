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
using Encog.Neural.Networks;
using Encog.Solve.Genetic.Species;

namespace Encog.Solve.Genetic
{
    /// <summary>
    /// Implements a genetic algorithm. This is an abstract class. Other classes are
    /// provided by Encog use this base class to train neural networks or
    /// provide an answer to the traveling salesman problem.
    ///
    /// The genetic algorithm is also capable of using a thread pool to speed
    /// execution.
    /// </summary>
    public class GeneticAlgorithm
    {
        /// <summary>
        /// Threadpool timeout.
        /// </summary>
        public const int TIMEOUT = 120;

        /// <summary>
        /// The score calculation object.
        /// </summary>
        public ICalculateGenomeScore CalculateScore { get; set; }

        /// <summary>
        /// Compare two genomes.
        /// </summary>
        public GenomeComparator Comparator { get; set; }

        /// <summary>
        /// Perform crossovers.
        /// </summary>
        public ICrossover Crossover { get; set; }

        /// <summary>
        /// The mating population.
        /// </summary>
        public double MatingPopulation { get; set; }

        /// <summary>
        /// Used to mutate offspring.
        /// </summary>
        private IMutate mutate;

        /// <summary>
        /// The percent that should mutate.
        /// </summary>
        public double MutationPercent { get; set; }

        /// <summary>
        /// What percent should be chosen to mate. They will choose partners from the
        /// entire mating population.
        /// </summary>
        public double PercentToMate { get; set; }

        /// <summary>
        /// The population.
        /// </summary>
        public IPopulation Population { get; set; }

        /// <summary>
        /// Perform a score calculation for the specified genome.
        /// </summary>
        /// <param name="g">The genome to calculate for.</param>
        public void PerformScoreCalculation(IGenome g)
        {
            ((IContextClearable)g.Organism).ClearContext();
            double score = CalculateScore.CalculateScore(g);
            g.Score = score;
        }

        /// <summary>
        /// The mutation object.
        /// </summary>
        public IMutate Mutate
        {
            get
            {
                return mutate;
            }
            set
            {
                this.mutate = value;
            }
        }



        /// <summary>
        /// Perform one generation.
        /// </summary>
        public virtual void Iteration()
        {

            int countToMate = (int)(Population.Genomes.Count * PercentToMate);
            int offspringCount = countToMate * 2;
            int offspringIndex = Population.Genomes.Count - offspringCount;
            int matingPopulationSize = (int)(Population.Genomes.Count * MatingPopulation);

            TaskGroup group = EncogConcurrency.Instance
                    .CreateTaskGroup();

            // mate and form the next generation
            for (int i = 0; i < countToMate; i++)
            {
                IGenome mother = Population.Genomes[i];
                int fatherInt = (int)(ThreadSafeRandom.NextDouble() * matingPopulationSize);
                IGenome father = Population.Genomes[fatherInt];
                IGenome child1 = Population.Genomes[offspringIndex];
                IGenome child2 = Population.Genomes[
                        offspringIndex + 1];

                MateWorker worker = new MateWorker(mother, father, child1,
                        child2);

                EncogConcurrency.Instance.ProcessTask(worker, group);

                offspringIndex += 2;
            }

            group.WaitForComplete();

            // sort the next generation
            Population.Sort();
        }

        /// <summary>
        /// Add a genome to a species.
        /// </summary>
        /// <param name="species">Tge soecues to add to.</param>
        /// <param name="genome">The genome to add.</param>
        public void AddSpeciesMember(ISpecies species, IGenome genome)
        {

            if (this.Comparator.IsBetterThan(genome.Score, species.BestScore))
            {
                species.BestScore = genome.Score;
                species.GensNoImprovement = 0;
                species.Leader = genome;
            }

            species.Members.Add(genome);

        }
    }
}