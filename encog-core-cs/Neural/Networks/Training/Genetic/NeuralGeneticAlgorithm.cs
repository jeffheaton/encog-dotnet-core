using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Solve.Genetic;
using Encog.MathUtil.Randomize;
using Encog.Solve.Genetic.Genome;
using Encog.Solve.Genetic.Population;
using Encog.Solve.Genetic.Crossover;
using Encog.Solve.Genetic.Mutate;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// Implements a genetic algorithm that allows a feedforward or simple
    /// recurrent neural network to be trained using a genetic algorithm. 
    /// 
    /// There are essentially two ways you can make use of this
    /// class.
    /// 
    /// Either way, you will need a score object.  The score object tells the
    /// genetic algorithm how well suited a neural network is.
    /// 
    /// If you would like to use genetic algorithms with a training set you 
    /// should make use TrainingSetScore class.  This score object uses a training
    /// set to score your neural network.
    /// 
    /// If you would like to be more abstract, and not use a training set, you
    /// can create your own implementation of the CalculateScore method.  This
    /// class can then score the networks any way that you like.
    /// </summary>
    public class NeuralGeneticAlgorithm : BasicTraining
    {
        /// <summary>
        /// Very simple class that implements a genetic algorithm.
        /// </summary>
        public class NeuralGeneticAlgorithmHelper : GeneticAlgorithm
        {
            /// <summary>
            /// The error from the last iteration.
            /// </summary>
            public double Error
            {
                get
                {
                    IGenome genome = this.Population.GetBest();
                    return genome.Score;
                }
            }

            /// <summary>
            /// The current best neural network.
            /// </summary>
            public BasicNetwork Network
            {
                get
                {
                    IGenome genome = this.Population.GetBest();
                    return (BasicNetwork)genome.Organism;
                }
            }

        }

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(NeuralGeneticAlgorithm));
#endif


        /// <summary>
        /// Simple helper class that implements the required methods to 
	    /// implement a genetic algorithm.
        /// </summary>
        public NeuralGeneticAlgorithmHelper Helper { get; set; }

        /// <summary>
        /// Construct a neural genetic algorithm. 
        /// </summary>
        /// <param name="network">The network to base this on.</param>
        /// <param name="randomizer">The randomizer used to create this initial population.</param>
        /// <param name="calculateScore">The score calculation object.</param>
        /// <param name="populationSize">The population size.</param>
        /// <param name="mutationPercent">The percent of offspring to mutate.</param>
        /// <param name="percentToMate">The percent of the population allowed to mate.</param>
        public NeuralGeneticAlgorithm(BasicNetwork network,
                IRandomizer randomizer,
                ICalculateScore calculateScore,
                int populationSize, double mutationPercent,
                double percentToMate)
        {

            this.Helper = new NeuralGeneticAlgorithmHelper();
            this.Helper.CalculateScore = new GeneticScoreAdapter(calculateScore);
            IPopulation population = new BasicPopulation(populationSize);
            Helper.MutationPercent = mutationPercent;
            Helper.MatingPopulation = (percentToMate * 2);
            Helper.PercentToMate = (percentToMate);
            Helper.Crossover = (new Splice(network.Structure.CalculateSize() / 3));
            Helper.Mutate = (new MutatePerturb(4.0));
            Helper.Population = (population);
            for (int i = 0; i < population.PopulationSize; i++)
            {
                BasicNetwork chromosomeNetwork = (BasicNetwork)network
                        .Clone();
                randomizer.Randomize(chromosomeNetwork);

                NeuralGenome genome =
                    new NeuralGenome(this, chromosomeNetwork);
                Helper.PerformScoreCalculation(genome);
                Helper.Population.Add(genome);
            }
            population.Sort();
        }


        /// <summary>
        /// The network that is being trained.
        /// </summary>
        public override BasicNetwork Network
        {
            get
            {
                return Helper.Network;
            }
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {

            if (logger.IsInfoEnabled)
            {
                logger.Info("Performing Genetic iteration.");
            }
            PreIteration();
            Helper.Iteration();
            Error = Helper.Error;
            PostIteration();
        }
    }
}
